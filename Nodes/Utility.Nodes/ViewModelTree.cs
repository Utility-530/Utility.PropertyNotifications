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
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes
{
    public class NodeViewModelConstants
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
        public static readonly INodeViewModel Current = default;
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
        public static readonly Visual Style = default;
        public static readonly VisualLayout ContainerStyle = default;
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

    public partial class NodeViewModel : Tree, INodeViewModel, ISet, IGet
    {
        private Collection collection;

        private bool? isHighlighted = NodeViewModelConstants.IsHighlighted;
        private bool isExpanded = NodeViewModelConstants.IsExpanded;
        private Arrangement arrangement = NodeViewModelConstants.Arrangement;
        private int column = NodeViewModelConstants.Column;
        private int row = NodeViewModelConstants.Row;
        private Orientation orientation = NodeViewModelConstants.Orientation;
        private bool? isVisible = NodeViewModelConstants.IsVisible;
        private bool isLoaded = NodeViewModelConstants.IsLoaded;
        private bool? isValid = NodeViewModelConstants.IsValid;
        private bool isReadOnly = NodeViewModelConstants.IsReadOnly;
        private bool isEditable = NodeViewModelConstants.IsEditable;
        private IViewModelTree current = NodeViewModelConstants.Current;
        private bool isClicked = NodeViewModelConstants.IsClicked;
        private bool isSelected = NodeViewModelConstants.IsSelected;
        private DateTime? removed = NodeViewModelConstants.Removed;
        private bool isReplicable = NodeViewModelConstants.IsReplicable;
        private bool isRemovable = NodeViewModelConstants.IsRemovable;
        private int order = NodeViewModelConstants.Order;
        private bool isAugmentable = NodeViewModelConstants.IsAugmetable;
        private bool? isContentVisible = NodeViewModelConstants.IsContentVisible;
        private Position2D connectorPosition = NodeViewModelConstants.ConnectorPosition;
        private string dataTemplate = NodeViewModelConstants.DataTemplate;
        private Visual style = NodeViewModelConstants.Style;
        private VisualLayout layOut = NodeViewModelConstants.ContainerStyle;
        private string selectedItemTemplate = NodeViewModelConstants.SelectedItemTemplate;
        private string itemsPanelTemplate = NodeViewModelConstants.ItemsPanelTemplate;
        private string title = NodeViewModelConstants.Title;
        private PointF location = NodeViewModelConstants.Location;
        private SizeF? size = NodeViewModelConstants.Size;
        private bool isActive = NodeViewModelConstants.IsActive;
        protected string name = NodeViewModelConstants.Name;
        private bool isChildrenRefreshed = NodeViewModelConstants.IsChildrenRefreshed;
        private object? value = NodeViewModelConstants.Value;

        public event Action? Closed;

        public bool isEnabled = NodeViewModelConstants.IsEnabled;
        protected bool isProliferable = NodeViewModelConstants.IsProliferable;
        protected bool isWithinWindowBounds = NodeViewModelConstants.IsWithinWindowBounds;


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

        public INodeViewModel Root
        {
            get => this.m_items.FirstOrDefault() as INodeViewModel;
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

        //TODO : Rename to Customisation/ or custom-style
        [FieldName(nameof(dataTemplate))]
        public string DataTemplate
        {
            get { RaisePropertyCalled(dataTemplate); return dataTemplate; }
            set => this.RaisePropertyReceived(ref this.dataTemplate, value);
        }

        public Visual Style
        {
            get { RaisePropertyCalled(style); return style; }
            set => this.RaisePropertyReceived(ref this.style, value);
        }

        public VisualLayout Layout
        {
            get { RaisePropertyCalled(layOut); return layOut; }
            set => this.RaisePropertyReceived(ref this.layOut, value);
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
                case nameof(Current): /*current = value as INodeViewModel;*/ break;
                case nameof(Name): name = value as string; break;
                case nameof(IsActive): isActive = Convert.ToBoolean(value); break;
                case nameof(IsEditable): isEditable = Convert.ToBoolean(value); break;
                case nameof(IsReadOnly): isReadOnly = Convert.ToBoolean(value); break;
                case nameof(IsVisible): isVisible = value as bool?; break;
                case nameof(IsValid): isValid = value as bool?; break;
                case nameof(IsHighlighted): isHighlighted = value as bool?; break;
                case nameof(IsClicked): isClicked = Convert.ToBoolean(value); break;
                case nameof(IsSelected): isSelected = Convert.ToBoolean(value); break;
                case nameof(IsExpanded): isExpanded = Convert.ToBoolean(value); break;
                case nameof(IsReplicable): isReplicable = Convert.ToBoolean(value); break;
                case nameof(IsRemovable): isRemovable = Convert.ToBoolean(value); break;
                case nameof(IsProliferable): isProliferable = Convert.ToBoolean(value); break;
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
                case nameof(IsAugmentable): this.isAugmentable = (bool)value; break;
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
                nameof(IsProliferable) => isProliferable,
                nameof(IsAugmentable) => isAugmentable,
                _ => throw new ArgumentException($"Unknown field: {name}")
            };
        }

        public virtual IEnumerable<(string PropertyName, object OriginalValue, object NewValue)> Changes()
        {
            if (current != NodeViewModelConstants.Current)
                yield return (nameof(Current), NodeViewModelConstants.Current, current);
            if (name != NodeViewModelConstants.Name)
                yield return (nameof(Name), NodeViewModelConstants.Name, name);
            if (isActive != NodeViewModelConstants.IsActive)
                yield return (nameof(IsActive), NodeViewModelConstants.IsActive, isActive);
            if (isEditable != NodeViewModelConstants.IsEditable)
                yield return (nameof(IsEditable), NodeViewModelConstants.IsEditable, isEditable);
            if (isReadOnly != NodeViewModelConstants.IsReadOnly)
                yield return (nameof(IsReadOnly), NodeViewModelConstants.IsReadOnly, isReadOnly);
            if (isVisible != NodeViewModelConstants.IsVisible)
                yield return (nameof(IsVisible), NodeViewModelConstants.IsVisible, isVisible);
            if (isValid != NodeViewModelConstants.IsValid)
                yield return (nameof(IsValid), NodeViewModelConstants.IsValid, isValid);
            if (isHighlighted != NodeViewModelConstants.IsHighlighted)
                yield return (nameof(IsHighlighted), NodeViewModelConstants.IsHighlighted, isHighlighted);
            if (isClicked != NodeViewModelConstants.IsClicked)
                yield return (nameof(IsClicked), NodeViewModelConstants.IsClicked, isClicked);
            if (isSelected != NodeViewModelConstants.IsSelected)
                yield return (nameof(IsSelected), NodeViewModelConstants.IsSelected, isSelected);
            if (isExpanded != NodeViewModelConstants.IsExpanded)
                yield return (nameof(IsExpanded), NodeViewModelConstants.IsExpanded, isExpanded);
            if (isReplicable != NodeViewModelConstants.IsReplicable)
                yield return (nameof(IsReplicable), NodeViewModelConstants.IsReplicable, isReplicable);
            if (isRemovable != NodeViewModelConstants.IsRemovable)
                yield return (nameof(IsRemovable), NodeViewModelConstants.IsRemovable, isRemovable);
            if (isChildrenRefreshed != NodeViewModelConstants.IsChildrenRefreshed)
                yield return (nameof(IsChildrenRefreshed), NodeViewModelConstants.IsChildrenRefreshed, isChildrenRefreshed);
            if (order != NodeViewModelConstants.Order)
                yield return (nameof(Order), NodeViewModelConstants.Order, order);
            if (row != NodeViewModelConstants.Row)
                yield return (nameof(Row), NodeViewModelConstants.Row, row);
            if (column != NodeViewModelConstants.Column)
                yield return (nameof(Column), NodeViewModelConstants.Column, column);
            if (removed != NodeViewModelConstants.Removed)
                yield return (nameof(Removed), NodeViewModelConstants.Removed, removed);
            if (arrangement != NodeViewModelConstants.Arrangement)
                yield return (nameof(Arrangement), NodeViewModelConstants.Arrangement, arrangement);
            if (orientation != NodeViewModelConstants.Orientation)
                yield return (nameof(Orientation), NodeViewModelConstants.Orientation, orientation);
            if (connectorPosition != NodeViewModelConstants.ConnectorPosition)
                yield return (nameof(ConnectorPosition), NodeViewModelConstants.ConnectorPosition, connectorPosition);
            if (location != NodeViewModelConstants.Location)
                yield return (nameof(Location), NodeViewModelConstants.Location, location);
            if (size != NodeViewModelConstants.Size)
                yield return (nameof(Size), NodeViewModelConstants.Size, size);
            if (dataTemplate != NodeViewModelConstants.DataTemplate)
                yield return (nameof(DataTemplate), NodeViewModelConstants.DataTemplate, dataTemplate);
            if (itemsPanelTemplate != NodeViewModelConstants.ItemsPanelTemplate)
                yield return (nameof(ItemsPanelTemplate), NodeViewModelConstants.ItemsPanelTemplate, itemsPanelTemplate);
            if (title != NodeViewModelConstants.Title)
                yield return (nameof(Title), NodeViewModelConstants.Title, title);
            if (!Equals(value, NodeViewModelConstants.Value))
                yield return (nameof(Value), NodeViewModelConstants.Value, value);
            if (isEnabled != NodeViewModelConstants.IsEnabled)
                yield return (nameof(IsEnabled), NodeViewModelConstants.IsEnabled, isEnabled);
            if (selectedItemTemplate != NodeViewModelConstants.SelectedItemTemplate)
                yield return (nameof(SelectedItemTemplate), NodeViewModelConstants.SelectedItemTemplate, selectedItemTemplate);
            if (isWithinWindowBounds != NodeViewModelConstants.IsWithinWindowBounds)
                yield return (nameof(IsWithinWindowBounds), NodeViewModelConstants.IsWithinWindowBounds, isWithinWindowBounds);
        }
    }


    public class Comparer : IComparer<object>
    {
        public int Compare(object? x, object? y)
        {
            return (x as NodeViewModel)?.Order.CompareTo((y as NodeViewModel)?.Order) ?? 0;
        }
    }
}