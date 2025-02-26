using Bogus.Bson;
using Newtonsoft.Json;
using Splat;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;
using Utility.Interfaces.Exs;
using Utility.Keys;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Repos;
using Utility.ViewModels;
using Utility.WPF;

namespace Utility.Nodes.Demo.Queries
{
    internal class MainViewModel : NotifyPropertyChangedBase
    {
        //private const string test = "../../../Data/test.json";
        private bool isInitialised = false;
        private ObservableCollection<FilterEntity> filters = new();
        private ObservableCollection<User> filteredUsers = new();
        private Lazy<User[]> users = new(() =>
        {
            string json = ResourceHelper.GetEmbeddedResource("Users.json").AsString();
            return JsonConvert.DeserializeObject<User[]>(json);
        });
        private Command refreshCommand, saveCommand;
        private ICollectionView collectionView;
        private FilterEntity filterEntity;
        private INode[] nodes;
        private Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());
        private Lazy<ILiteRepository> repo = new(() => Locator.Current.GetService<ILiteRepository>());
        private Dictionary<FilterEntity, bool> _checked = new();

        public MainViewModel()
        {
            FinishEdit = new Command<object>(o =>
            {
                if (o is NewObjectRoutedEventArgs { IsAccepted: true } args)
                {
                    filters.Add(args.NewObject as FilterEntity);
                }
            });

            CheckedCommand = new Command<object>(o =>
            {
                if (o is RoutedEventArgs { Source: CheckBox { DataContext: FilterEntity dataContext, IsChecked: bool b } source } routedEventArgs)
                {
                    _checked[dataContext] = b;

                }
                refresh();
            });
        }

        public ICommand SaveCommand { get; }

        public INode[] Nodes => nodes;

        public FilterEntity Filter
        {
            get => filterEntity;
            set
            {
                Save();
                filterEntity = value;
                find(filterEntity)
                    .Subscribe(a =>
                    {
                        nodes = [a];
                        RaisePropertyChanged(nameof(Nodes));
                    });

            }
        }

        public IEnumerable Filtered => filteredUsers;

        public ICollectionView Filters
        {
            get
            {
                if (isInitialised == false)
                {
                    initialise();
                    isInitialised = true;
                }

                return collectionView;
            }
        }


        public ICommand FinishEdit { get; }

        public ICommand CheckedCommand { get; }

        public User[] Users => users.Value;

        public ICommand RefreshCommand
        {
            get
            {
                return refreshCommand ??= new Command(() =>
                {
                    //Users = users.Value.Where(Filter.ToObject<TreeFilter>()).ToArray();
                    //this.RaisePropertyChanged(nameof(Users));
                });
            }
        }

        public void Save()
        {
            if (filterEntity != null)
            {
                filterEntity.Body = JsonConvert.SerializeObject(nodes[0]);
            }
        }

        private void refresh()
        {
            filteredUsers.Clear();
            
            foreach (var filter in _checked)
            {
                if (filter.Value && string.IsNullOrEmpty(filter.Key.Body) == false)
                {

                    find(filter.Key).Subscribe(a =>
                    {
                        foreach (var user in users.Value)
                        {
                            if (a.Data is AndOrModel andOrModel)
                            {
                                if (andOrModel.Get(user))
                                {
                                    filteredUsers.Add(user);
                                }
                            }
                        }
                    });
                }
            }
        }


        private IObservable<INode> find(FilterEntity filter)
        {
            return find(filter, () =>
            {
                var model = new AndOrModel { Name = "and_or" };
                var node = new Node(model) { IsExpanded = true, Key = new GuidKey(filter.Id), Orientation = Enums.Orientation.Vertical };
                return node;
            });

            IObservable<INode> find(FilterEntity filter, Func<INode> factory)
            {
                if (string.IsNullOrEmpty(filter.Body))
                {
                    var node = factory();
                    source.Value.Add(node);
                    return Observable.Return(node);
                }
                else
                {
                    return Observable.Create<INode>(observer =>
                    {
                        INode? node = null;
                        return source.Value
                        .Single(new GuidKey(filter.Id))
                        .Subscribe(a =>
                        {
                            node = a;
                        },
                        () =>
                        {
                            node ??= JsonConvert.DeserializeObject<Node>(filter.Body);
                            source.Value.Add(node);
                            observer.OnNext(node);
                        });
                    });
                }
            }

        }

        private void initialise()
        {
            initialiseCollectionView();
            monitorUpdates();
            monitorCollectionChanges();

            void monitorUpdates()
            {
                repo.Value
                    .All()
                    .ToObservable()
                    .Subscribe(r =>
                    {
                        foreach (FilterEntity entity in r)
                        {
                            entity.WithChanges().Subscribe(a =>
                            {
                                repo.Value.Update(entity);
                            });
                            Application.Current.Dispatcher.BeginInvoke(() => filters.Add(entity));
                        }
                    }, () =>
                    {
                        RaisePropertyChanged();
                    });
            }

            void monitorCollectionChanges()
            {
                filters
                    .Changes()
                    .Subscribe(a =>
                    {
                        if (a.Type == Changes.Type.Add)
                        {
                            repo.Value.Update(a.Value);
                        }
                        else if (a.Type == Changes.Type.Remove)
                        {
                            repo.Value.Remove(a.Value);
                            // remove from INodeSource
                        }
                    });

            }
            void initialiseCollectionView()
            {
                collectionView = CollectionViewSource.GetDefaultView(filters);
                (collectionView as ListCollectionView).IsLiveGrouping = true;
                PropertyGroupDescription groupDescription = new(nameof(FilterEntity.GroupKey));
                collectionView.GroupDescriptions.Add(groupDescription);
            }
        }


    }
}
