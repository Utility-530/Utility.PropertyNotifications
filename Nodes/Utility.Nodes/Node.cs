using Jellyfish;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.Changes;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.PropertyNotifications;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class Node : ViewModelTree, INode, /*IGuid,*/ /*IName,*/ IIsEditable, IIsExpanded, IIsPersistable, IIsVisible //, INode, 
    {
        object data;

        public Node(object data) : this()
        {
            Data = data;
        }

        public Node()
        {
            AddCommand = new RelayCommand(async a =>
            {
                this.IsExpanded = true;
                var node = await ToTree(a);
                Add(node);
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
                IsPersistable = true
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
                if (data is IGuid guid)
                {
                    this.Key = new GuidKey(guid.Guid);
                }
                if (data is IChildren children)
                {
                    this.WithChangesTo(a => a.IsExpanded)
                        .Where(a => a)
                        .Subscribe(a =>
                        {
                            children.Children.Subscribe(async a =>
                            {

                                if (a is Change { Type: Changes.Type.Add, Value: { } value })
                                {

                                    this.m_items.Add(await ToTree(value));
                                }
                            });
                        });
                }

                RaisePropertyChanged(ref previousValue, value);
            }
        }

        public override ITree Parent
        {
            get => parent; set
            {
                RaisePropertyChanged(ref parent, value);
            }
        }


        protected override void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<Tree>())
                {
                    if (item.Parent == null)
                        item.Parent = this;
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
