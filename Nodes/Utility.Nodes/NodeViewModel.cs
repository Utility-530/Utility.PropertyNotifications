using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using Utility.Interfaces;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Keys;
using Utility.Nodify.Base;
using Utility.Trees;
using Utility.Trees.Abstractions;

namespace Utility.Nodes
{
    public partial class NodeViewModel : Tree, INodeViewModel
    {
        private object data;
        private ICollection<IConnectorViewModel> inputs = new ObservableCollection<IConnectorViewModel>();
        private ICollection<IConnectorViewModel> outputs = new ObservableCollection<IConnectorViewModel>();
        private RangeObservableCollection<INodeViewModel> nodes = [];

        public NodeViewModel(object data) : this()
        {
            Data = data;
        }

        public NodeViewModel()
        {
            AddCommand = new Command<object>(async a =>
            {
                var node = a is IReadOnlyTree ? a : await ToTree(a);
                Add(node);
                this.IsExpanded = true;
            });

            RemoveCommand = new Command<object>(a =>
            {
                (this.Parent() as ITree)?.Remove(a);
            });

            EditCommand = new Command<object>(a =>
            {
                if (a != null)
                    this.Data = a;
            });

            AddParentCommand = new Command<object>(async a =>
            {
                NodeViewModel node = (NodeViewModel)(await ToTree(a));
                node.Add(this);
            });
            nodes.WhenAdded(x => x.Diagram = this);
        }

        public ICommand AddCommand { get; init; }
        public ICommand RemoveCommand { get; init; }
        public ICommand EditCommand { get; init; }
        public ICommand AddParentCommand { get; init; }

        public override Task<ITree> ToTree(object? value)
        {
            throw new Exception("ToTree is obsolete");
            var node = new NodeViewModel(value)
            {
                Parent = this,
                Inputs = [],
                Outputs = []
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
                if (Key != null && SuppressExceptions == false && SuppressKeyException == false)
                {
                    throw new Exception($"Key {Key} not null!");
                }
                base.Key = value;
            }
        }

        public INodeViewModel Diagram { get; set; }

        public virtual ICollection<IConnectorViewModel> Inputs
        {
            get =>
                inputs;
            set
            {
                inputs = value;
                foreach (var inp in value)
                {
                    addInput(inp);
                }
            }
        }

        private void addInput(IConnectorViewModel x)
        {
            x.Node = this;
            x.IsInput = true;
        }


        public virtual ICollection<IConnectorViewModel> Outputs
        {
            get => outputs;
            set
            {
                outputs = value;
            }
        }

        public virtual ICollection<IConnectionViewModel> Connections
        {
            get; set;
        }

        public virtual ICollection<INodeViewModel> Nodes => nodes;

        public bool SuppressExceptions { get; set; }
        public bool SuppressKeyException { get; set; }
        public NodeState State { get; set; }

        public static NodeViewModel Create(object? data, object[] items, string? key = null)
        {
            NodeViewModel node = null;

            if (data != null)
                node = new NodeViewModel(data) { Key = key ?? new GuidKey() };
            else
                node = new NodeViewModel() { Key = key ?? new GuidKey() };
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