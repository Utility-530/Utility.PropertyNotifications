using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public class Node : ViewModelTree, INode, IIsEditable, IIsExpanded, IIsPersistable
    {
        object data;

        public Node(object data) : this()
        {
            Data = data;
        }

        public Node()
        {
            //IsExpanded = true;
            AddCommand = new Command<object>(async a =>
            {
                var node = await ToTree(a);
                Add(node);
                this.IsExpanded = true;
            });

            RemoveCommand = new Command<object>(a =>
            {
                Parent.Remove(this);
            });

            EditCommand = new Command<object>(a =>
            {
                if (a != null)
                    this.Data = a;
            });

            AddParentCommand = new Command<object>(async a =>
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
            get => data;
            set
            {
                if (data == value && SuppressExceptions == false)
                {
                    throw new Exception("vdfs 3332222kjj");
                }
                if (data != value)
                {
                    var previous = data;
                    data = value;
                    RaisePropertyChanged(previous, value);
                }
            }
        }

        public override string Key
        {
            get => base.Key; set
            {
                if (Key != null && SuppressExceptions == false)
                {
                    throw new Exception($"Key {Key} not null!");
                }
                base.Key = value;
            }
        }

        public override ITree Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    var previous = parent;
                    parent = value;
                    RaisePropertyChanged(previous, value);
                }
            }
        }

        public bool SuppressExceptions { get; set; }    

        public static Node Create(object? data, object[] items, string? key = null)
        {
            Node node = null;

            if (data != null)
                node = new Node(data) { Key = key ?? new GuidKey() };
            else
                node = new Node() { Key = key ?? new GuidKey() };
            node.SuppressExceptions = true;
            if (items.Any())
                node.Add(items);
            return node;
        }

        protected override void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {

        }
    }
}
