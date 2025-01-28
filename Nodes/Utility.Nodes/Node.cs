using Jellyfish;
using Splat;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Windows.Input;
using Utility.Changes;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes.Common;
using Utility.Nodes.Filters;
using Utility.PropertyNotifications;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class Node : ViewModelTree, INode, IIsEditable, IIsExpanded, IIsPersistable
    {
        object data;

        Lazy<INodeSource> source = new(() => Locator.Current.GetService<INodeSource>());

        public Node(object data) : this()
        {
            Data = data;
        }

        public Node()
        {
            AddCommand = new RelayCommand(async a =>
            {
                var node = await ToTree(a);
                Add(node);
                this.IsExpanded = true;
            });

            RemoveCommand = new RelayCommand(a =>
            Parent.Remove(this));

            EditCommand = new RelayCommand(a =>
            {
                if (a != null)
                    this.Data = a;
            });

            AddParentCommand = new RelayCommand(async a =>
            {
                Node node = (Node)(await ToTree(a));
                node.Add(this);
            });
        }
        public ICommand AddCommand { get; init; }
        public ICommand RemoveCommand { get; init; }
        public ICommand EditCommand { get; init; }
        public ICommand AddParentCommand { get; init; }

        public override Task<ITree> ToTree(object value)
        {
            var node = new Node(value)
            {
                Parent = this,
            };
            return Task.FromResult((ITree)node);
        }

        public override object Data
        {
            get => data; set
            {
                if (data == value)
                {
                    throw new Exception("vdfs 3332222kjj");
                }
                var previousValue = data;
                data = value;
                if (data is ISetNode iSetNode)
                {
                    iSetNode.SetNode(this);
                }
                if (data is IGuid guid && this.Key == null)
                {
                    this.Key = new GuidKey(guid.Guid);
                }

                RaisePropertyChanged(ref previousValue, value);
            }
        }

        public override string Key
        {
            get => base.Key; set
            {
                if (Key != null)
                {
                    throw new Exception($"Key {Key} not null!");
                }
                base.Key = value;

                this.WithChangesTo(a => a.Data)
                    .Where(a => a is not string)
                    .Take(1)
                    .Subscribe(data =>
                    {
                        //if (data is IDescriptor && data is ICount)
                        //{

                        //}
                        if (data is IChildren children)
                        {
                            if (data is IHasChildren { HasChildren: false } hasChildren)
                            {

                            }
                            else
                                _children(children, Guid.Parse(value))
                                    .Filter(this.WithChangesTo(a => a.IsExpanded))
                                    .Subscribe(async a =>
                                    {
                                        if (a is Change { Type: Changes.Type.Add, Value: { } value })
                                        {
                                            if (value is INode node)
                                                this.m_items.Add(node);
                                            else
                                                this.m_items.Add(await ToTree(value));
                                        }
                                        else if (a is Change { Type: Changes.Type.Remove, Value: { } _value })
                                        {
                                            this.m_items.RemoveBy(c => (c as IData).Data == _value);
                                        }
                                    });
                        }
                    });

                IObservable<object> _children(IChildren children, Guid guid)
                {
                    return Observable.Create<object>(observer =>
                    {
                        return source.Value
                        .ChildrenByGuidAsync(guid)
                        .Subscribe(a =>
                        {
                            if (a.Data?.ToString() == source.Value.New || data is ICount)
                            {
                                children.Children.Subscribe(a => observer.OnNext(a));
                            }
                            else if (a.Data != null && m_items.Any(n => ((IKey)n).Key == a.Key) == false)
                            {
                                observer.OnNext(a);
                            }
                            else
                            {

                            }
                        });
                    });
                }
            }
        }

        public override ITree Parent
        {
            get => parent;
            set
            {
                RaisePropertyChanged(ref parent, value);
            }
        }


        protected override void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<INode>())
                {
                    item.Parent ??= this;
                    if (item.Key != default)
                        source.Value.Add(item);
                    else
                    {

                    }
                }
            }
            if (args.Action == NotifyCollectionChangedAction.Remove && args.OldItems != null)
            {
                foreach (var item in args.OldItems.Cast<Tree>())
                {
                    item.Parent = null;
                }
            }
            else if (args.Action != NotifyCollectionChangedAction.Move && args.OldItems != null)
            {
                foreach (var item in args.OldItems.Cast<Tree>())
                {
                    //item.Parent = null;
                    //item.ResetOnCollectionChangedEvent();
                }
            }
            this.InvokeCollectionChanged(sender, args);
        }
    }
}
