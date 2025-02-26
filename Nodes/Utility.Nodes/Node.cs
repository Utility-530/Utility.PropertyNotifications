using Jellyfish;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
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
            AddCommand = new RelayCommand(async a =>
            {
                var node = await ToTree(a);
                Add(node);
                this.IsExpanded = true;
            });

            RemoveCommand = new RelayCommand(a =>
            {
                Parent.Remove(this);
            });

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
            get => data;
            set
            {
                if (data == value)
                {
                    throw new Exception("vdfs 3332222kjj");
                }
                var previousValue = data;
                data = value;            
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

        }
    }
}
