using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Utility.Collections;
using Utility.Enums;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;
using Utility.Trees;

namespace Utility.Nodes
{

    public class ViewModelTree : Tree, IIsEditable, IIsExpanded, IIsPersistable, IIsVisible, IRemoved, IIsReadOnly, ISortOrder, ISort, IDataTemplate
    {
        private bool? isHighlighted;
        private bool isExpanded = false;
        private Arrangement arrangement;
        private int column;
        private int row;
        private Orientation orientation;
        private bool? isVisible = true;
        private bool? isValid = null;
        private bool isReadOnly;
        private bool isEditable;
        private INode current;
        private bool isClicked;
        private bool isSelected;
        private DateTime? removed;
        private bool isReplicable;
        private bool isRemovable;
        private int order;
        private Collection collection;
        private bool isAugmentable;
        private bool? isContentVisible = true;
        private Position2D connectorPosition = Position2D.None;
        private string dataTemplate;
        private string itemsPanelTemplate;

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
            collection = new Collection();
            collection.CollectionChanged += ItemsOnCollectionChanged;
            collection.Comparer = new Comparer();
            return collection;
        }

        public static string Field(string propertyName) => propertyName switch { nameof(Current) => nameof(current), _ => throw new NotImplementedException("vc3333") };

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

        public bool IsAugmentable
        {
            get => isAugmentable; set
            {
                RaisePropertyChanged(ref isAugmentable, value);
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

        public Position2D ConnectorPosition
        {
            get => connectorPosition;
            set
            {
                RaisePropertyChanged(ref connectorPosition, value);
            }
        }

        public ObservableCollection<Dimension> Columns { get; set; } = new();

        public ObservableCollection<Dimension> Rows { get; set; } = new();

        public int Row
        {
            get => row;
            set
            {
                RaisePropertyChanged(ref row, value);
            }
        }

        public int Column
        {
            get => column;
            set
            {
                RaisePropertyChanged(ref column, value);
            }
        }

        public int Order
        {
            get => order;
            set
            {
                RaisePropertyChanged(ref order, value);
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
             
        public bool? IsContentVisible
        {
            get => isContentVisible;
            set
            {
                RaisePropertyChanged(ref isContentVisible, value);
            }
        }

        public bool IsReplicable
        {
            get => isReplicable;
            set
            {
                RaisePropertyChanged(ref isReplicable, value);
            }
        }

        public bool IsRemovable
        {
            get => isRemovable;
            set
            {
                RaisePropertyChanged(ref isRemovable, value);
            }
        }

        public string DataTemplate
        {
            get => dataTemplate;
            set
            {
                RaisePropertyChanged(ref dataTemplate, value);
            }
        }

        public string ItemsPanelTemplate
        {
            get => itemsPanelTemplate;
            set
            {
                RaisePropertyChanged(ref itemsPanelTemplate, value);
            }
        }

        public bool IsPersistable { get; set; }

        public ObservableCollection<Exception> Errors { get; set; } = new();

        public INode Current
        {
            get => current;
            set
            {
                if (current != value)
                {
                    var previousValue = current;
                    if (current != null)
                        current.IsPersistable = false;
                    if (value != null)
                        value.IsPersistable = true;
                    current = value;
                    RaisePropertyChanged(previousValue, value);                        
                }
            }
        }

        public bool Sort(object? o = null)
        {
            collection.Sort();
            return true;
        }

        public override string ToString()
        {
            return Data?.ToString();
        }
    }


    public class Comparer : IComparer<object>
    {
        public int Compare(object? x, object? y)
        {
            return (x as ViewModelTree)?.Order.CompareTo((y as ViewModelTree)?.Order) ?? 0;
        }
    }
}
