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
using Utility.Interfaces;
using Utility.Structs;
using Utility.Trees;

namespace Utility.Nodes
{
    public class ViewModelTreeConstants
    {
        public static readonly bool? IsHighlighted = default;
        public static readonly bool IsExpanded = default;
        public static readonly Arrangement Arrangement = default;
        public static readonly int Column = default;
        public static readonly int Row = default;
        public static readonly Orientation Orientation = Orientation.None;
        public static readonly bool IsVisible = true;
        public static readonly bool IsLoaded = default;
        public static readonly bool? IsValid = null;
        public static readonly bool IsReadOnly = default;
        public static readonly bool IsEditable = default;
        public static readonly IViewModelTree Current = default;
        public static readonly bool IsClicked = default;
        public static readonly bool IsSelected = default;
        public static readonly DateTime? Removed = default;
        public static readonly bool IsReplicable = default;
        public static readonly bool IsRemovable = default;
        public static readonly int Order = default;
        public static readonly bool IsAugmetable = default;
        public static readonly bool? IsContentVisible = true;
        public static readonly Position2D ConnectorPosition = Position2D.None;
        public static readonly string DataTemplate = default;
        public static readonly string SelectedItemTemplate = default;
        public static readonly string ItemsPanelTemplate = default;
        public static readonly string Title = default;
        public static readonly PointF Location = default;
        public static readonly SizeF Size = default;
        public static readonly bool IsActive = false;
        public static readonly string Name = default;
        public static readonly bool IsChildrenRefreshed = false;
        public static readonly object? Value = default;
        public static readonly bool IsEnabled = true;
        public static readonly bool IsProliferable = true;
        public static readonly bool IsWithinWindowBounds = false;
    }

    public class ViewModelTree : Tree, IViewModelTree, ISet, IGet
    {
        private bool? isHighlighted = ViewModelTreeConstants.IsHighlighted;
        private bool isExpanded = ViewModelTreeConstants.IsExpanded;
        private Arrangement arrangement = ViewModelTreeConstants.Arrangement;
        private int column = ViewModelTreeConstants.Column;
        private int row = ViewModelTreeConstants.Row;
        private Orientation orientation = ViewModelTreeConstants.Orientation;
        private bool? isVisible = ViewModelTreeConstants.IsVisible;
        private bool isLoaded = ViewModelTreeConstants.IsLoaded;
        private bool? isValid = ViewModelTreeConstants.IsValid;
        private bool isReadOnly = ViewModelTreeConstants.IsReadOnly;
        private bool isEditable = ViewModelTreeConstants.IsEditable;
        private IViewModelTree current = ViewModelTreeConstants.Current;
        private bool isClicked = ViewModelTreeConstants.IsClicked;
        private bool isSelected = ViewModelTreeConstants.IsSelected;
        private DateTime? removed = ViewModelTreeConstants.Removed;
        private bool isReplicable = ViewModelTreeConstants.IsReplicable;
        private bool isRemovable = ViewModelTreeConstants.IsRemovable;
        private int order = ViewModelTreeConstants.Order;
        private Collection collection;
        private bool isAugmentable = ViewModelTreeConstants.IsAugmetable;
        private bool? isContentVisible = ViewModelTreeConstants.IsContentVisible;
        private Position2D connectorPosition = ViewModelTreeConstants.ConnectorPosition;
        private string dataTemplate = ViewModelTreeConstants.DataTemplate;
        private string selectedItemTemplate = ViewModelTreeConstants.SelectedItemTemplate;
        private string itemsPanelTemplate = ViewModelTreeConstants.ItemsPanelTemplate;
        private string title = ViewModelTreeConstants.Title;
        private PointF location = ViewModelTreeConstants.Location;
        private SizeF? size = ViewModelTreeConstants.Size;
        private bool isActive = ViewModelTreeConstants.IsActive;
        protected string name = ViewModelTreeConstants.Name;
        private bool isChildrenRefreshed = ViewModelTreeConstants.IsChildrenRefreshed;
        private object? value = ViewModelTreeConstants.Value;

        public event Action? Closed;

        public bool isEnabled = ViewModelTreeConstants.IsEnabled;
        protected bool isProliferable = ViewModelTreeConstants.IsProliferable;
        protected bool isWithinWindowBounds = ViewModelTreeConstants.IsWithinWindowBounds;

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
            set => this.RaisePropertyChanged(ref this.isSelected, value);
        }

        public bool IsExpanded
        {
            get { RaisePropertyCalled(isExpanded); return isExpanded; }
            set
            {
                this.RaisePropertyReceived(ref this.isExpanded, value);
            }
        }

        public bool IsLoaded
        {
            get { RaisePropertyCalled(isLoaded); return isLoaded; }
            set => this.RaisePropertyChanged(ref this.isLoaded, value);
        }

        public bool? IsVisible
        {
            get { RaisePropertyCalled(isVisible); return isVisible; }
            set
            {
                this.RaisePropertyChanged(ref this.isVisible, value);

                if (value == false)
                {
                    Closed?.Invoke();
                }
            }
        }

        [YieldAttribute]
        public bool IsWithinWindowBounds
        {
            get { RaisePropertyCalled(isWithinWindowBounds); return isWithinWindowBounds; }
            set => this.RaisePropertyChanged(ref this.isWithinWindowBounds, value);
        }

        public bool IsEnabled
        {
            get { RaisePropertyCalled(isEnabled); return isEnabled; }
            set => this.RaisePropertyReceived(ref this.isEnabled, value);
        }

        public Arrangement Arrangement
        {
            get { RaisePropertyCalled(arrangement); return arrangement; }
            set => this.RaisePropertyReceived(ref this.arrangement, value);
        }

        public Orientation Orientation
        {
            get { RaisePropertyCalled(orientation); return orientation; }
            set
            {
                if (this.orientation != value)
                {
                    this.RaisePropertyReceived(ref this.orientation, value);
                    this.RaisePropertyChanged();
                }
            }
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

        [FieldName(nameof(selectedItemTemplate))]
        public string SelectedItemTemplate
        {
            get { RaisePropertyCalled(selectedItemTemplate); return selectedItemTemplate; }
            set => this.RaisePropertyReceived(ref this.selectedItemTemplate, value);
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
            get { RaisePropertyCalled(current); return current; }
            set => this.RaisePropertyReceived(ref this.current, value);
        }

        public bool IsChildrenRefreshed
        {
            get { RaisePropertyCalled(isChildrenRefreshed); return isChildrenRefreshed; }
            set => this.RaisePropertyReceived(ref this.isChildrenRefreshed, value);
        }

        public bool ShouldValueBeTracked { get; set; } = true;
        public bool DoesValueRequireSaving { get; set; }
        public bool DoesValueRequireLoading { get; set; }
        public bool IsValueLoaded { get; set; }
        public bool AreChildrenLoaded { get; set; }
        public bool AreChildrenTracked { get; set; }
        public bool IsProliferable
        {
            get { RaisePropertyCalled(isProliferable); return isProliferable; }
            set => this.RaisePropertyReceived(ref this.isProliferable, value);
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
                case nameof(SelectedItemTemplate): this.selectedItemTemplate = (string)value; break;
                case nameof(IsWithinWindowBounds): this.isWithinWindowBounds = (bool)value; break;
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
                nameof(SelectedItemTemplate) => selectedItemTemplate,
                nameof(IsWithinWindowBounds) => isWithinWindowBounds,
                _ => throw new ArgumentException($"Unknown field: {name}")
            };
        }

        public virtual IEnumerable<(string PropertyName, object OriginalValue, object NewValue)> Changes()
        {
            if (current != ViewModelTreeConstants.Current)
                yield return (nameof(Current), ViewModelTreeConstants.Current, current);
            if (name != ViewModelTreeConstants.Name)
                yield return (nameof(Name), ViewModelTreeConstants.Name, name);
            if (isActive != ViewModelTreeConstants.IsActive)
                yield return (nameof(IsActive), ViewModelTreeConstants.IsActive, isActive);
            if (isEditable != ViewModelTreeConstants.IsEditable)
                yield return (nameof(IsEditable), ViewModelTreeConstants.IsEditable, isEditable);
            if (isReadOnly != ViewModelTreeConstants.IsReadOnly)
                yield return (nameof(IsReadOnly), ViewModelTreeConstants.IsReadOnly, isReadOnly);
            if (isVisible != ViewModelTreeConstants.IsVisible)
                yield return (nameof(IsVisible), ViewModelTreeConstants.IsVisible, isVisible);
            if (isContentVisible != ViewModelTreeConstants.IsContentVisible)
                yield return (nameof(IsContentVisible), ViewModelTreeConstants.IsContentVisible, isContentVisible);
            if (isValid != ViewModelTreeConstants.IsValid)
                yield return (nameof(IsValid), ViewModelTreeConstants.IsValid, isValid);
            if (isHighlighted != ViewModelTreeConstants.IsHighlighted)
                yield return (nameof(IsHighlighted), ViewModelTreeConstants.IsHighlighted, isHighlighted);
            if (isClicked != ViewModelTreeConstants.IsClicked)
                yield return (nameof(IsClicked), ViewModelTreeConstants.IsClicked, isClicked);
            if (isSelected != ViewModelTreeConstants.IsSelected)
                yield return (nameof(IsSelected), ViewModelTreeConstants.IsSelected, isSelected);
            if (isExpanded != ViewModelTreeConstants.IsExpanded)
                yield return (nameof(IsExpanded), ViewModelTreeConstants.IsExpanded, isExpanded);
            if (isReplicable != ViewModelTreeConstants.IsReplicable)
                yield return (nameof(IsReplicable), ViewModelTreeConstants.IsReplicable, isReplicable);
            if (isRemovable != ViewModelTreeConstants.IsRemovable)
                yield return (nameof(IsRemovable), ViewModelTreeConstants.IsRemovable, isRemovable);
            if (isChildrenRefreshed != ViewModelTreeConstants.IsChildrenRefreshed)
                yield return (nameof(IsChildrenRefreshed), ViewModelTreeConstants.IsChildrenRefreshed, isChildrenRefreshed);
            if (order != ViewModelTreeConstants.Order)
                yield return (nameof(Order), ViewModelTreeConstants.Order, order);
            if (row != ViewModelTreeConstants.Row)
                yield return (nameof(Row), ViewModelTreeConstants.Row, row);
            if (column != ViewModelTreeConstants.Column)
                yield return (nameof(Column), ViewModelTreeConstants.Column, column);
            if (removed != ViewModelTreeConstants.Removed)
                yield return (nameof(Removed), ViewModelTreeConstants.Removed, removed);
            if (arrangement != ViewModelTreeConstants.Arrangement)
                yield return (nameof(Arrangement), ViewModelTreeConstants.Arrangement, arrangement);
            if (orientation != ViewModelTreeConstants.Orientation)
                yield return (nameof(Orientation), ViewModelTreeConstants.Orientation, orientation);
            if (connectorPosition != ViewModelTreeConstants.ConnectorPosition)
                yield return (nameof(ConnectorPosition), ViewModelTreeConstants.ConnectorPosition, connectorPosition);
            if (location != ViewModelTreeConstants.Location)
                yield return (nameof(Location), ViewModelTreeConstants.Location, location);
            if (size != ViewModelTreeConstants.Size)
                yield return (nameof(Size), ViewModelTreeConstants.Size, size);
            if (dataTemplate != ViewModelTreeConstants.DataTemplate)
                yield return (nameof(DataTemplate), ViewModelTreeConstants.DataTemplate, dataTemplate);
            if (itemsPanelTemplate != ViewModelTreeConstants.ItemsPanelTemplate)
                yield return (nameof(ItemsPanelTemplate), ViewModelTreeConstants.ItemsPanelTemplate, itemsPanelTemplate);
            if (title != ViewModelTreeConstants.Title)
                yield return (nameof(Title), ViewModelTreeConstants.Title, title);
            if (!Equals(value, ViewModelTreeConstants.Value))
                yield return (nameof(Value), ViewModelTreeConstants.Value, value);
            if (isEnabled != ViewModelTreeConstants.IsEnabled)
                yield return (nameof(IsEnabled), ViewModelTreeConstants.IsEnabled, isEnabled);
            if (selectedItemTemplate != ViewModelTreeConstants.SelectedItemTemplate)
                yield return (nameof(SelectedItemTemplate), ViewModelTreeConstants.SelectedItemTemplate, selectedItemTemplate);
            if (isWithinWindowBounds != ViewModelTreeConstants.IsWithinWindowBounds)
                yield return (nameof(IsWithinWindowBounds), ViewModelTreeConstants.IsWithinWindowBounds, isWithinWindowBounds);
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