using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Utility.Commands;
using Utility.Extensions;
using Utility.Helpers.Ex;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Nodes.Meta;
using Utility.Structs.Repos;
using Utility.WPF.Demo.Common.ViewModels;
using Utility.WPF.Reactives;
using Utility.Interfaces;

namespace Utility.WPF.Demo.Trees
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomSelectStyleUserControl : UserControl
    {
        NodeEngine engine = new NodeEngine(new Repository(), new ValueRepository(), new DataActivator(), null, new NodeInterface(), new NodeSource());


        public CustomSelectStyleUserControl()
        {
            InitializeComponent();
            build();
            this.Loaded += CustomStyleUserControl_Loaded;

            void CustomStyleUserControl_Loaded(object sender, RoutedEventArgs e)
            {
                var collection = new ObservableCollection<ButtonViewModel>();
                ItemsControl.ItemsSource = collection;
                foreach (var item in Initialise())
                {
                    collection.Add(item);
                }
                IEnumerable<ButtonViewModel> Initialise()
                {
                    var _collection = TreeViewHelper.VisibleItems(MyTreeView).ToCollection(out _);
                    var foo = new Uri("/Utility.WPF.Controls.Trees;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                    var resourceDictionary = new ResourceDictionary() { Source = foo };

                    foreach (var style in FindResourcesByType(resourceDictionary, typeof(Utility.WPF.Controls.Trees.CustomTreeViewItem)).ToArray())
                    {
                        yield return new ButtonViewModel
                        {
                            Header = style.Key,
                            Command = new Command(() =>
                            {
                                MyTreeView.ItemContainerStyleSelector = null;
                                CustomTreeItemContainerStyleSelector.Instance.Current = style.Value;
                                MyTreeView.ItemContainerStyleSelector = CustomTreeItemContainerStyleSelector.Instance;

                                foreach (var item in _collection.ToArray())
                                {
                                    item.ItemContainerStyleSelector = null;
                                    item.ItemContainerStyleSelector = CustomTreeItemContainerStyleSelector.Instance;
                                }
                            })
                        };
                    }
                    IEnumerable<KeyValuePair<string?, Style>> FindResourcesByType(ResourceDictionary resources, Type type)
                    {
                        return resources.MergedDictionaries.SelectMany(d => FindResourcesByType(d, type)).Union(resources
                            .Cast<DictionaryEntry>()
                            .Where(s => s.Value is Style style && style.TargetType == type)
                            .Select(a => new KeyValuePair<string?, Style>(a.Key?.ToString(), (Style)a.Value)));
                    }
                }
            }
        }


        void build()
        {
            var node = new Model(() =>
            [
                new Model(() =>
                [
                    new Model(){
                        Name="a.1",
                        IsProliferable=false
                    },
                    new Model(){
                        Name="a.2",
                        IsProliferable=false
                    },
                ])
                {
                    Name = "a",
                    IsProliferable=false
                }
            ])
            {
                Guid = Guid.NewGuid(),
                Name = "Root",
                IsExpanded = true,
                IsProliferable = false
            };

            engine.Create(node).Subscribe(a =>
            {
                MyTreeView.ItemsSource = new INodeViewModel[] { a };
            });
        }
    }


    public class CustomTreeItemContainerStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            return Current;

            //return base.SelectStyle(item, container);
        }

        public Style Current { get; set; }

        public static CustomTreeItemContainerStyleSelector Instance { get; } = new();
    }

    public class Repository : ITreeRepository
    {
        public IEnumerable<Duplication> Duplicate(Guid oldGuid, Guid? newParentGuid = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Changes.Set<Structs.Repos.Key>> Find(Guid? parentGuid = null, string? name = null, Guid? guid = null, Type? type = null, int? index = null)
        {
            return Observable.Return<Changes.Set<Structs.Repos.Key>>(new(
                [new Changes.Change<Structs.Repos.Key>(new Structs.Repos.Key(Guid.NewGuid(), parentGuid.Value, typeof(Model), name, default, default), default, Changes.Type.Add)]));
        }

        public IObservable<Structs.Repos.Key> FindRecursive(Guid parentGuid, int? maxIndex = null)
        {
            throw new NotImplementedException();
        }

        public IObservable<Structs.Repos.Key?> InsertRoot(Guid guid, string name, Type type)
        {
            return Observable.Return<Structs.Repos.Key?>(new Structs.Repos.Key(guid, default, null, name, null, null));
        }

        public int? MaxIndex(Guid parentGuid, string? name = null)
        {
            throw new NotImplementedException();
        }

        public DateTime Remove(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void UpdateName(Guid parentGuid, Guid guid, string name, string newName)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueRepository : IValueRepository
    {
        public void Copy(Guid guid, Guid newGuid)
        {
            throw new NotImplementedException();
        }

        public IObservable<DateValue> Get(Guid guid, string? name = null)
        {
            return Observable.Return(new DateValue(guid, name, DateTime.Now, null));
        }

        public void Set(Guid guid, string name, object value, DateTime dateTime)
        {
            //throw new NotImplementedException();
        }


    }

    public class DataActivator : IDataActivator
    {
        public object Activate(Structs.Repos.Key? a)
        {
            throw new Exception("DSsd");
        }
    }

    public class NodeSource : INodeSource
    {
        Collection<INodeViewModel> nodes = new();
        public IObservable<INodeViewModel> this[string key] => throw new NotImplementedException();

        public IReadOnlyCollection<INodeViewModel> Nodes => nodes;

        public void Add(INodeViewModel node)
        {
            nodes.Add(node);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public INodeViewModel? Find(string key)
        {
            return nodes.SingleOrDefault(a => a.Key() == key);
        }

        public bool KeyExistsInCode(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}