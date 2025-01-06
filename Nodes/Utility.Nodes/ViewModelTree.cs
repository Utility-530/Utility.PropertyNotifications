using System.Reactive.Linq;
using Utility.Enums;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Trees;

namespace Utility.Nodes
{

    public class ViewModelTree : ObservableTree, IGuid, IName, IIsEditable, IIsExpanded, IIsPersistable, IIsVisible //, INode, 
    {

        private bool? isHighlighted;
        private bool isExpanded;
        private string itemsPanelKey;
        private Arrangement arrangement;
        private int columns;
        private int rows;
        private Orientation orientation;
        private bool? isVisible = true;
        private bool? isValid = null;
        private object data;
        private bool isEditable;
        //private ObservableCollection<Node> items;
        private ViewModelTree currentNode;
        //private ViewModelTree selectedNode;
        private Guid guid;
        private bool isClicked;
        private bool isSelected;
        //int? _index;

        public ViewModelTree(string name, object data) : this()
        {
            Name = name;
            Data = data;
        }

        public ViewModelTree()
        {
      
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
   
        public Node Root
        {
            get => this.m_items.FirstOrDefault() as Node;
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

        public ViewModelTree Current
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
      

        public override string ToString()
        {
            return Data?.ToString();
        }

        public override bool Equals(object? obj)
        {
            if (obj is IGuid node)
            {
                return node.Guid == this.Guid;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }

        public bool Equals(ViewModelTree? obj)
        {
            if (obj is IGuid node)
            {
                return node.Guid == this.Guid;
            }
            return false;
        }
    }
}
