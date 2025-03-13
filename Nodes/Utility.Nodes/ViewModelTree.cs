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

    public class ViewModelTree : Tree, IIsEditable, IIsExpanded, IIsPersistable, IIsVisible, IRemoved, IIsReadOnly
    {

        private bool? isHighlighted;
        private bool isExpanded = false;
        private Arrangement arrangement;
        private int columns;
        private int rows;
        private Orientation orientation;
        private bool? isVisible = true;
        private bool? isValid = null;
        private bool isReadOnly;
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

                RaisePropertyChanged(ref removed, value);

            }
        }

        public bool? IsValid
        {
            get => isValid; set
            {

                RaisePropertyChanged(ref isValid, value);

            }
        }

        // relates to the ability to modify the value
        public override bool IsReadOnly
        {
            get => isReadOnly;
            set
            {
                RaisePropertyChanged(ref isReadOnly, value);
            }
        }

        // relates to the ability to add/remove nodes 
        public bool IsEditable
        {
            get => isEditable; set
            {
                RaisePropertyChanged(ref isEditable, value);
            }
        }


        public bool? IsHighlighted
        {
            get => isHighlighted;
            set
            {

                RaisePropertyChanged(ref isHighlighted, value);

            }
        }

        public bool IsClicked
        {
            get => isClicked; set
            {
                RaisePropertyChanged(ref isClicked, value);
            }
        }

        public bool IsSelected
        {
            get => isSelected; set
            {
                RaisePropertyChanged(ref isSelected, value);
            }
        }

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                RaisePropertyChanged(ref isExpanded, value);
            }
        }

        public Arrangement Arrangement
        {
            get => arrangement;
            set
            {
                RaisePropertyChanged(ref arrangement, value);
            }
        }

        public Orientation Orientation
        {
            get => orientation;
            set
            {
                RaisePropertyChanged(ref orientation, value);
            }
        }

        public int Rows
        {
            get => rows;
            set
            {
                RaisePropertyChanged(ref rows, value);
            }
        }

        public int Columns
        {
            get => columns;
            set
            {
                RaisePropertyChanged(ref columns, value);
            }
        }

        public bool? IsVisible
        {
            get => isVisible;
            set
            {
                RaisePropertyChanged(ref isVisible, value);
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
                    var previousValue = currentNode;
                    if (currentNode != null)
                        currentNode.IsPersistable = false;
                    if (value != null)
                        value.IsPersistable = true;
                    currentNode = value;
                    RaisePropertyChanged(ref previousValue, value);
                }
            }
        }


        public override string ToString()
        {
            return Data?.ToString();
        }
    }
}
