using AutoMapper;
using Jellyfish;
using Splat;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.Changes;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Repos;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Filters
{
    public interface ISetNode
    {
        void SetNode(Node node);
    }

    public class Node : ViewModelTree, IGuid, IName, IIsEditable, IIsExpanded, IIsPersistable, IIsVisible //, INode, 
    {
        object data;

        public Node(string name, object data) : this()
        {
            Name = name;
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

            AddParentCommand = new RelayCommand(a =>
            {
                var node = new Node(Name + "_parent", a) { Parent = this };
                node.Add(this);
            });
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand AddParentCommand { get; }

        public override Task<ITree> ToTree(object value)
        {
         
            var node = new Node(Name + "_child", value)
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
                data = value;
                if (data is ISetNode iSetNode)
                {
                    iSetNode.SetNode(this);
                }
                if (data is IGuid guid)
                {
                    this.Guid = guid.Guid;
                }
                if (data is IChildren children)
                {
                    this.WithChangesTo(a => a.IsExpanded)
                        .Where(a => a)
                        .Subscribe(a =>
                        {
                            children.Children.Subscribe(async a =>
                            {

                                if (a is Change { Type: Changes.Type.Add, Value:{ } value })
                                {

                                    this.m_items.Add(await ToTree(value));
                                }
                            });
                        });
                }

                RaisePropertyChanged();
            }
        }

        public override ITree Parent
        {
            get => parent; set
            {
                if (parent?.Equals(value) == true)
                    return;
                parent = value;



                RaisePropertyChanged();
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


        public override string ToString()
        {
            return Data?.ToString();
        }


        public override bool Equals(ITree other)
        {
            if (other is ViewModelTree node)
                return node.Equals(this);
            else
                return base.Equals(other);
        }


    }
}
