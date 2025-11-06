using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reactive.Linq;
using Utility.Attributes;
using Utility.Collections;
using Utility.Enums;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Structs;
using Utility.Trees;

namespace Utility.Nodes
{
    public class ViewModelTree : Tree, IViewModelTree, ISet, IGet
    {
        private bool? isHighlighted;
        private bool isExpanded = true;
        private Arrangement arrangement;
        private int column;
        private int row;
        private Orientation orientation;
        private bool? isVisible = true;
        private bool? isValid = null;
        private bool isReadOnly;
        private bool isEditable;
        private IViewModelTree current;
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
        private string title;
        private PointF location;
        private SizeF? size;
        private bool isActive;
        protected string name;
        private bool isChildrenRefreshed;
        private object? value;

        public event Action? Closed;

        public bool isEnabled = true;

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

        //public static string Field(string propertyName) => propertyName switch { nameof(Current) => nameof(current), _ => throw new NotImplementedException("vc3333") };

        public virtual Guid Guid { get => Guid.TryParse(Key, out var guid) ? guid : default; set => Key = value.ToString(); }

        public virtual bool IsChildrenTracked { get; set; } = true;
        public int? LocalIndex { get; set; }

        public string GroupKey { get; set; }

        public override object? Value
        {
            get { _ = RaisePropertyCalled(value); return value; }
            set => this.RaisePropertyReceived(ref this.value, value);
        }

        public IViewModelTree Root
        {
            get => this.m_items.FirstOrDefault() as IViewModelTree;
            set => this.Add(value);
        }

        public bool IsActive
        {
            get { RaisePropertyCalled(isActive); return isActive; }
            set => this.RaisePropertyReceived(ref this.isActive, value);
        }

        public bool IsConnectorsReversed { get; set; }

        public virtual string Name
        {
            get { RaisePropertyCalled(name); return name; }
            set => this.RaisePropertyReceived(ref this.name, value);
        }

        public PointF Location
        {
            get { RaisePropertyCalled(location); return location; }
            set => this.RaisePropertyReceived(ref this.location, value);
        }

        public SizeF? Size
        {
            get { RaisePropertyCalled(size); return size; }
            set => this.RaisePropertyReceived(ref this.size, value);
        }

        public DateTime? Removed
        {
            get { RaisePropertyCalled(removed); return removed; }
            set => this.RaisePropertyReceived(ref this.removed, value);
        }

        public bool? IsValid
        {
            get { RaisePropertyCalled(isValid); return isValid; }
            set => this.RaisePropertyReceived(ref this.isValid, value);
        }

        // relates to the ability to modify the value
        public override bool IsReadOnly
        {
            get { RaisePropertyCalled(isReadOnly); return isReadOnly; }
            set => this.RaisePropertyReceived(ref this.isReadOnly, value);
        }

        // relates to the ability to add/remove nodes
        public bool IsEditable
        {
            get { RaisePropertyCalled(isEditable); return isEditable; }
            set => this.RaisePropertyReceived(ref this.isEditable, value);
        }

        public bool IsAugmentable
        {
            get { RaisePropertyCalled(isAugmentable); return isAugmentable; }
            set => this.RaisePropertyReceived(ref this.isAugmentable, value);
        }

        public bool? IsHighlighted
        {
            get { RaisePropertyCalled(isHighlighted); return isHighlighted; }
            set => this.RaisePropertyReceived(ref this.isHighlighted, value);
        }

        public bool IsClicked
        {
            get { RaisePropertyCalled(isClicked); return isClicked; }
            set => this.RaisePropertyReceived(ref this.isClicked, value);
        }

        public bool IsSelected
        {
            get { RaisePropertyCalled(isSelected); return isSelected; }
            set => this.RaisePropertyReceived(ref this.isSelected, value);
        }
         
        public bool IsExpanded
        {
            get { RaisePropertyCalled(isExpanded); return isExpanded; }
            set => this.RaisePropertyReceived(ref this.isExpanded, value);
        }

        public Arrangement Arrangement
        {
            get { RaisePropertyCalled(arrangement); return arrangement; }
            set => this.RaisePropertyReceived(ref this.arrangement, value);
        }

        public Orientation Orientation
        {
            get { RaisePropertyCalled(orientation); return orientation; }
            set => this.RaisePropertyReceived(ref this.orientation, value);
        }

        [FieldName(nameof(connectorPosition))]
        public Position2D ConnectorPosition
        {
            get { RaisePropertyCalled(connectorPosition); return connectorPosition; }
            set => this.RaisePropertyReceived(ref this.connectorPosition, value);
        }

        public ObservableCollection<Dimension> Columns { get; set; } = new();

        public ObservableCollection<Dimension> Rows { get; set; } = new();

        public int Row
        {
            get { RaisePropertyCalled(row); return row; }
            set => this.RaisePropertyReceived(ref this.row, value);
        }

        public int Column
        {
            get { RaisePropertyCalled(column); return column; }
            set => this.RaisePropertyReceived(ref this.column, value);
        }

        public int Order
        {
            get { RaisePropertyCalled(order); return order; }
            set => this.RaisePropertyReceived(ref this.order, value);
        }

        public bool? IsVisible
        {
            get { RaisePropertyCalled(isVisible); return isVisible; }
            set
            {
                this.RaisePropertyReceived(ref this.isVisible, value);

                if (value == false)
                {
                    Closed?.Invoke();
                }
            }
        }

        public bool IsEnabled
        {
            get { RaisePropertyCalled(isEnabled); return isEnabled; }
            set => this.RaisePropertyReceived(ref this.isEnabled, value);
        }

        public bool? IsContentVisible
        {
            get { RaisePropertyCalled(isContentVisible); return isContentVisible; }
            set => this.RaisePropertyReceived(ref this.isContentVisible, value);
        }

        public bool IsReplicable
        {
            get { RaisePropertyCalled(isReplicable); return isReplicable; }
            set => this.RaisePropertyReceived(ref this.isReplicable, value);
        }

        public bool IsRemovable
        {
            get { RaisePropertyCalled(isRemovable); return isRemovable; }
            set => this.RaisePropertyReceived(ref this.isRemovable, value);
        }

        [FieldName(nameof(dataTemplate))]
        public string DataTemplate
        {
            get { RaisePropertyCalled(dataTemplate); return dataTemplate; }
            set => this.RaisePropertyReceived(ref this.dataTemplate, value);
        }

        [FieldName(nameof(itemsPanelTemplate))]
        public string ItemsPanelTemplate
        {
            get { RaisePropertyCalled(itemsPanelTemplate); return itemsPanelTemplate; }
            set => this.RaisePropertyReceived(ref this.itemsPanelTemplate, value);
        }


        public string Title
        {
            get { RaisePropertyCalled(title); return title; }
            set => this.RaisePropertyReceived(ref this.title, value);
        }

        public virtual bool IsSingular => false;

        public ObservableCollection<Exception> Errors { get; set; } = new();

        public IViewModelTree Current
        {
            get { RaisePropertyCalled(title); return current; }
            set => this.RaisePropertyReceived(ref this.current, value);
        }

        public bool IsChildrenRefreshed
        {
            get { RaisePropertyCalled(isChildrenRefreshed); return isChildrenRefreshed; }
            set => this.RaisePropertyReceived(ref this.isChildrenRefreshed, value);
        }

        public bool IsValueTracked { get; set; } = true;
        public bool IsValueSaved { get; set; }
        public bool IsValueLoaded { get; set; }

        public bool Sort(object? o = null)
        {
            collection.Sort();
            return true;
        }

        public override string ToString()
        {
            return Data?.ToString();
        }

        public virtual void Set(object value, string name)
        {
            switch (name)
            {
                case nameof(Current): /*current = value as IViewModelTree;*/ break;
                case nameof(Name): name = value as string; break;
                case nameof(IsActive): isActive = Convert.ToBoolean(value); break;
                case nameof(IsEditable): isEditable = Convert.ToBoolean(value); break;
                case nameof(IsReadOnly): isReadOnly = Convert.ToBoolean(value); break;
                case nameof(IsVisible): isVisible = value as bool?; break;
                case nameof(IsContentVisible): isContentVisible = value as bool?; break;
                case nameof(IsValid): isValid = value as bool?; break;
                case nameof(IsHighlighted): isHighlighted = value as bool?; break;
                case nameof(IsClicked): isClicked = Convert.ToBoolean(value); break;
                case nameof(IsSelected): isSelected = Convert.ToBoolean(value); break;
                case nameof(IsExpanded): isExpanded = Convert.ToBoolean(value); break;
                case nameof(IsReplicable): isReplicable = Convert.ToBoolean(value); break;
                case nameof(IsRemovable): isRemovable = Convert.ToBoolean(value); break;
                case nameof(IsChildrenRefreshed): isChildrenRefreshed = Convert.ToBoolean(value); break;
                case nameof(Order): order = Convert.ToInt32(value); break;
                case nameof(Row): row = Convert.ToInt32(value); break;
                case nameof(Column): column = Convert.ToInt32(value); break;
                case nameof(Removed): removed = value as DateTime?; break;
                case nameof(Arrangement): arrangement = (Arrangement)value; break;
                case nameof(Orientation): orientation = (Orientation)value; break;
                case nameof(ConnectorPosition): connectorPosition = (Position2D)value; break;
                case nameof(Location): location = (PointF)value; break;
                case nameof(Size): size = (SizeF)value; break;
                case nameof(DataTemplate): dataTemplate = value as string; break;
                case nameof(ItemsPanelTemplate): itemsPanelTemplate = value as string; break;
                case nameof(Title): title = value as string; break;
                case nameof(Value): this.value = value; break;
                case nameof(IsEnabled): this.isEnabled = (bool)value; break;
                default: throw new ArgumentException($"Unknown field: {name}");
            }
            RaisePropertyChanged(name);
        }

        public virtual object? Get(string name)
        {
            return name switch
            {
                nameof(Current) => current,
                nameof(Name) => name,
                nameof(IsActive) => isActive,
                nameof(IsEditable) => isEditable,
                nameof(IsReadOnly) => isReadOnly,
                nameof(IsVisible) => isVisible,
                nameof(IsContentVisible) => isContentVisible,
                nameof(IsValid) => isValid,
                nameof(IsHighlighted) => isHighlighted,
                nameof(IsClicked) => isClicked,
                nameof(IsSelected) => isSelected,
                nameof(IsExpanded) => isExpanded,
                nameof(IsReplicable) => isReplicable,
                nameof(IsRemovable) => isRemovable,
                nameof(IsChildrenRefreshed) => isChildrenRefreshed,
                nameof(Order) => order,
                nameof(Row) => row,
                nameof(Column) => column,
                nameof(Removed) => removed,
                nameof(Arrangement) => arrangement,
                nameof(Orientation) => orientation,
                nameof(ConnectorPosition) => connectorPosition,
                nameof(Location) => location,
                nameof(Size) => size,
                nameof(DataTemplate) => dataTemplate,
                nameof(ItemsPanelTemplate) => itemsPanelTemplate,
                nameof(Title) => title,
                nameof(Value) => value,
                nameof(IsEnabled) => isEnabled,
                _ => throw new ArgumentException($"Unknown field: {name}")
            };
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