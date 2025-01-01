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

    public class Node : Tree, IGuid, IName, IIsEditable, IIsExpanded, IIsPersistable, IIsVisible //, INode, 
    {

        private bool? isHighlighted;
        private bool isExpanded;
        private ITree parent;
        private string itemsPanelKey;
        private Arrangement arrangement;
        private int columns;
        private int rows;
        private Orientation orientation;
        private bool? isVisible = true;
        private bool? isValid = null;
        private object data;
        private bool isEditable;
        private ObservableCollection<Node> items;
        private Node currentNode;
        private Node selectedNode;
        private Guid guid;
        private bool isClicked;
        private bool isSelected;
        //int? _index;

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

        public string Name { get; set; }

        public int? LocalIndex { get; set; }

        public Guid Guid
        {
            get => guid; set
            {
                if (value != guid)
                {
                    guid = value;
                    RaisePropertyChanged(nameof(Guid));
                }
            }
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

        public Node Root
        {
            get => this.items.FirstOrDefault();
            set => this.Add(value);
        }

        public bool? IsValid
        {
            get => isValid; set
            {
                if (value != isValid)
                {
                    isValid = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsEditable
        {
            get => isEditable; set
            {
                if (value != isEditable)
                {
                    isEditable = value;
                    RaisePropertyChanged();
                }
            }
        }


        public bool? IsHighlighted
        {
            get => isHighlighted;
            set
            {
                if (value != isHighlighted)
                {
                    isHighlighted = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public bool IsClicked
        {
            get => isClicked; set
            {
                isClicked = value;
                RaisePropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => isSelected; set
            {
                isSelected = value;
                RaisePropertyChanged();
            }
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (value != isExpanded)
                {
                    isExpanded = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public Arrangement Arrangement
        {
            get => arrangement;
            set
            {
                if (value != arrangement)
                {
                    arrangement = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public Orientation Orientation
        {
            get => orientation;
            set
            {
                if (value != orientation)
                {
                    orientation = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public int Rows
        {
            get => rows;
            set
            {
                if (value != rows)
                {
                    rows = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public int Columns
        {
            get => columns;
            set
            {
                if (value != columns)
                {
                    columns = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public bool? IsVisible
        {
            get => isVisible;
            set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public bool IsPersistable { get; set; }

        public Node Current
        {
            get => currentNode;
            set
            {
                if (currentNode != value)
                {
                    if (currentNode != null)
                        currentNode.IsPersistable = false;
                    value.IsPersistable = true;
                    currentNode = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Node Selected
        {
            get => selectedNode;
            set
            {
                if (selectedNode != value)
                {
                    selectedNode = value;
                    RaisePropertyChanged();
                }
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

        public override bool Equals(object? obj)
        {
            if (obj is Node node)
            {
                return node.Guid == this.Guid;
            }
            return base.Equals(obj);
        }

        public override bool Equals(ITree other)
        {
            if (other is Node node)
                return Equals(node);
            else
                return base.Equals(other);
        }


        public bool Equals(Node? obj)
        {
            if (obj is Node node)
            {
                return node.Guid == this.Guid;
            }
            return false;
        }

        internal void Load()
        {
            TreeRepository.Instance
                .Get(this.Guid)
                .Subscribe(guid =>
                {
                    if (guid.Value is NodeDTO node)
                        Splat.Locator.Current.GetService<IMapper>()?.Map(node, this);
                });

        }
    }
}
