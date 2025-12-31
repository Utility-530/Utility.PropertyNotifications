using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Interfaces.Exs;
using Utility.WPF.Panels;
using Position2D = Utility.Enums.Position2D;

namespace Utility.WPF.Controls.Trees
{
    public partial class CustomTreeViewItem : TreeViewItem
    {
        public static readonly DependencyProperty AddCommandProperty = DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty TreeViewProperty = DependencyProperty.Register("TreeView", typeof(TreeView), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty AddParentCommandProperty = DependencyProperty.Register("AddParentCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(bool?), typeof(CustomTreeViewItem), new PropertyMetadata(null));
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata(false, IsEditingChanged));
        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register("IsValid", typeof(bool?), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsShowingProperty = DependencyProperty.Register("IsShowing", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata(false));
        public static readonly DependencyProperty NodeModelsProperty = DependencyProperty.Register("NodeModels", typeof(IEnumerable), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty ChildrenSelectorProperty = DependencyProperty.Register("ChildrenSelector", typeof(IChildrenSelector), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty NodeContainerStyleSelectorProperty = DependencyProperty.Register("NodeContainerStyleSelector", typeof(StyleSelector), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty SplitButtonStyleProperty = DependencyProperty.Register("SplitButtonStyle", typeof(Style), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty NodeContainerStyleProperty = DependencyProperty.Register("NodeContainerStyle", typeof(Style), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty ItemsPresenterVisibilityProperty = DependencyProperty.Register("ItemsPresenterVisibility", typeof(Visibility), typeof(CustomTreeViewItem), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(CustomTreeViewItem), new PropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(double), typeof(CustomTreeViewItem), new PropertyMetadata(0d));
        public static readonly DependencyProperty IsReplicableProperty = DependencyProperty.Register("IsReplicable", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsRemovableProperty = DependencyProperty.Register("IsRemovable", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty ArrangementProperty = DependencyProperty.Register("Arrangement", typeof(Utility.Enums.Arrangement), typeof(CustomTreeViewItem), new PropertyMetadata(_changed));
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(IEnumerable), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(IEnumerable), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsAugmentableProperty = DependencyProperty.Register("IsAugmentable", typeof(bool), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty ErrorsProperty = DependencyProperty.Register("Errors", typeof(IEnumerable), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty ConnectorPositionProperty = DependencyProperty.Register("ConnectorPosition", typeof(Position2D), typeof(CustomTreeViewItem), new PropertyMetadata(Position2D.None));
        public static readonly DependencyProperty ItemsPanelTemplateProperty = DependencyProperty.Register("ItemsPanelTemplate", typeof(string), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsContentVisibleProperty = DependencyProperty.Register(nameof(IsContentVisible), typeof(bool?), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty CheckedCommandProperty = DependencyProperty.Register("CheckedCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty SwapCommandProperty = DependencyProperty.Register("SwapCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty UnCheckedCommandProperty = DependencyProperty.Register("UnCheckedCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty CheckedPropertyNameProperty = DependencyProperty.Register("CheckedPropertyName", typeof(string), typeof(CustomTreeViewItem), new PropertyMetadata("IsChecked", isCheckedChanged));
        public static readonly DependencyProperty FinishEditCommandProperty = DependencyProperty.Register("FinishEditCommand", typeof(ICommand), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty EditConverterProperty = DependencyProperty.Register("EditConverter", typeof(IValueConverter), typeof(CustomTreeViewItem), new PropertyMetadata(new EditConverter()));
        public static readonly DependencyProperty MaxHeightOfEditBoxProperty = DependencyProperty.Register("MaxHeightOfEditBox", typeof(double), typeof(CustomTreeViewItem), new PropertyMetadata(300.0));


        public static readonly DependencyProperty NodeItemsSourceProperty = DependencyProperty.Register("NodeItemsSource", typeof(IEnumerable), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty NodeTemplateProperty = DependencyProperty.Register("NodeTemplate", typeof(DataTemplate), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty NodeTemplateSelectorProperty = DependencyProperty.Register("NodeTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTreeViewItem), new PropertyMetadata());


        public static readonly DependencyProperty EditProperty = DependencyProperty.Register("Edit", typeof(object), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty EditTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty EditTemplateSelectorProperty = DependencyProperty.Register("EditTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTreeViewItem), new PropertyMetadata());



        public static readonly DependencyProperty IsDropDownOpenProperty = ComboBox.IsDropDownOpenProperty.AddOwner(typeof(CustomTreeViewItem), new FrameworkPropertyMetadata(
        BooleanBoxes.FalseBox,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        new PropertyChangedCallback(OnIsDropDownOpenChanged),
        new CoerceValueCallback(CoerceIsDropDownOpen)));

        public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(typeof(CustomTreeViewItem));
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty = DependencyProperty.Register("SelectedItemTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty SelectedItemTemplateProperty = DependencyProperty.Register("SelectedItemTemplate", typeof(DataTemplate), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty IsReadOnlyProperty = ComboBox.IsReadOnlyProperty.AddOwner(typeof(CustomTreeViewItem));


        public static readonly RoutedEvent FinishEditEvent = EventManager.RegisterRoutedEvent(
    name: "FinishEdit",
    routingStrategy: RoutingStrategy.Bubble,
    handlerType: typeof(FinishEditRoutedEventHandler),
    ownerType: typeof(CustomTreeViewItem));

        public static readonly RoutedEvent ChangeEvent = EventManager.RegisterRoutedEvent(
          name: "Change",
          routingStrategy: RoutingStrategy.Bubble,
          handlerType: typeof(ChangeRoutedEventHandler),
          ownerType: typeof(CustomTreeViewItem));

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            name: "SelectionChanged",
        routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(System.Windows.Controls.SelectionChangedEventHandler),
            ownerType: typeof(CustomTreeViewItem));



        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...


        private static void _changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #region Properties

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public TreeView TreeView
        {
            get { return (TreeView)GetValue(TreeViewProperty); }
            set { SetValue(TreeViewProperty, value); }
        }

        public Position2D ConnectorPosition
        {
            get { return (Position2D)GetValue(ConnectorPositionProperty); }
            set { SetValue(ConnectorPositionProperty, value); }
        }

        public IEnumerable Errors
        {
            get { return (IEnumerable)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }

        public bool IsAugmentable
        {
            get { return (bool)GetValue(IsAugmentableProperty); }
            set { SetValue(IsAugmentableProperty, value); }
        }

        public bool? IsContentVisible
        {
            get { return (bool)GetValue(IsContentVisibleProperty); }
            set { SetValue(IsContentVisibleProperty, value); }
        }

        public Visibility ItemsPresenterVisibility
        {
            get { return (Visibility)GetValue(ItemsPresenterVisibilityProperty); }
            set { SetValue(ItemsPresenterVisibilityProperty, value); }
        }

        public bool IsShowing
        {
            get { return (bool)GetValue(IsShowingProperty); }
            set { SetValue(IsShowingProperty, value); }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public bool? IsValid
        {
            get { return (bool?)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public DataTemplate EditTemplate
        {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
        }

        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public ICommand AddParentCommand
        {
            get { return (ICommand)GetValue(AddParentCommandProperty); }
            set { SetValue(AddParentCommandProperty, value); }
        }

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public bool? IsHighlighted
        {
            get { return (bool?)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        public IEnumerable NodeModels
        {
            get { return (IEnumerable)GetValue(NodeModelsProperty); }
            set { SetValue(NodeModelsProperty, value); }
        }

        public DataTemplate NodeTemplate
        {
            get { return (DataTemplate)GetValue(NodeTemplateProperty); }
            set { SetValue(NodeTemplateProperty, value); }
        }

        public DataTemplateSelector NodeTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(NodeTemplateSelectorProperty); }
            set { SetValue(NodeTemplateSelectorProperty, value); }
        }

        public DataTemplateSelector EditTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(EditTemplateSelectorProperty); }
            set { SetValue(EditTemplateSelectorProperty, value); }
        }

        public IChildrenSelector ChildrenSelector
        {
            get { return (IChildrenSelector)GetValue(ChildrenSelectorProperty); }
            set { SetValue(ChildrenSelectorProperty, value); }
        }

        public StyleSelector NodeContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(NodeContainerStyleSelectorProperty); }
            set { SetValue(NodeContainerStyleSelectorProperty, value); }
        }

        public Style SplitButtonStyle
        {
            get { return (Style)GetValue(SplitButtonStyleProperty); }
            set { SetValue(SplitButtonStyleProperty, value); }
        }

        public Style NodeContainerStyle
        {
            get { return (Style)GetValue(NodeContainerStyleProperty); }
            set { SetValue(NodeContainerStyleProperty, value); }
        }

        public IEnumerable NodeItemsSource
        {
            get { return (IEnumerable)GetValue(NodeItemsSourceProperty); }
            set { SetValue(NodeItemsSourceProperty, value); }
        }

        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public bool IsReplicable
        {
            get { return (bool)GetValue(IsReplicableProperty); }
            set { SetValue(IsReplicableProperty, value); }
        }

        public bool IsRemovable
        {
            get { return (bool)GetValue(IsRemovableProperty); }
            set { SetValue(IsRemovableProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public Utility.Enums.Arrangement Arrangement
        {
            get { return (Utility.Enums.Arrangement)GetValue(ArrangementProperty); }
            set { SetValue(ArrangementProperty, value); }
        }

        public string ItemsPanelTemplate
        {
            get { return (string)GetValue(ItemsPanelTemplateProperty); }
            set { SetValue(ItemsPanelTemplateProperty, value); }
        }


        public object Edit
        {
            get { return (object)GetValue(EditProperty); }
            set { SetValue(EditProperty, value); }
        }

        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Used to allow custom binding for Selection.
        /// If Selection was set directly then no way to set it to child property of the selected item
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        public DataTemplateSelector SelectedItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedItemTemplateSelectorProperty); }
            set { SetValue(SelectedItemTemplateSelectorProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }


        #endregion Properties

        #region events

        public event FinishEditRoutedEventHandler FinishEdit
        {
            add { AddHandler(FinishEditEvent, value); }
            remove { RemoveHandler(FinishEditEvent, value); }
        }

        public event ChangeRoutedEventHandler Change
        {
            add { AddHandler(ChangeEvent, value); }
            remove { RemoveHandler(ChangeEvent, value); }
        }

        public event System.Windows.Controls.SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        #endregion events


        private static void IsEditingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}