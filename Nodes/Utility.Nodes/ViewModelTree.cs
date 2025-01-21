using System.Collections;
using System.Reactive.Linq;
using Utility.Collections;
using Utility.Enums;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Trees;

namespace Utility.Nodes
{

    public class ViewModelTree : Tree, /*IGuid,*/ /*IName, */IIsEditable, IIsExpanded, IIsPersistable, IIsVisible, IRemoved //, INode, 
    {

        private bool? isHighlighted;
        private bool isExpanded = true;
        private Arrangement arrangement;
        private int columns;
        private int rows;
        private Orientation orientation;
        private bool? isVisible = true;
        private bool? isValid = null;
        private bool isEditable;
        private INode currentNode;
        private bool isClicked;
        private bool isSelected;
        private DateTime? removed;

        public ViewModelTree(/*string name,*/ object data) : this()
        {
            //Name = name;
            Data = data;
        }

        public ViewModelTree()
        {
      
        }

        protected override IList CreateChildren()
        {
            var collection = new Collection();
            collection.CollectionChanged += ItemsOnCollectionChanged;
            return collection;
        }


        //public string Name { get; set; }

        public int? LocalIndex { get; set; }

        public INode Root
        {
            get => this.m_items.FirstOrDefault() as INode;
            set => this.Add(value);
        }

        public DateTime? Removed
        {
            get => removed; set
            {
                if (value != removed)
                {
                    removed = value;
                    RaisePropertyChanged();
                }
            }
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

        public INode Current
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
    }
}
