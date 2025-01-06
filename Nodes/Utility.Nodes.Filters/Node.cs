using AutoMapper;
using Jellyfish;
using Splat;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using Utility.Enums;
using Utility.Interfaces.NonGeneric;
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
            AddCommand = new RelayCommand(a =>
            {
                this.IsExpanded = true;
                var index = TreeRepository.Instance.MaxIndex(this.Guid, Name + "_child") ?? 0;
                var node = new Node(Name + "_child", a)
                {
                    LocalIndex = index + 1,
                    //IsEditable = true,
                };
                node.Parent = this;
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

        public override object Data
        {
            get => data; set
            {
                data = value;
                if (data is ISetNode iSetNode)
                {
                    iSetNode.SetNode(this);
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

                if (value == null)
                {
                    NodeSource.Instance.Nodes.Remove(this);
                    return;
                }
                NodeSource.Instance.Nodes.Add(this);
                TreeRepository.Instance
                    .Find((value as IGuid).Guid, this.Name, typeof(object), this.LocalIndex)
                    .Subscribe(guid =>
                    {
                        Guid = guid.Guid;
                    });

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
