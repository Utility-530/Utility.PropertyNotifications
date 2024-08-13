using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Utility.WPF.Panels;

namespace Utility.WPF.Controls.Trees
{
    public class ComboTreeViewItem : TreeViewItem, ISelection
    {

        static ComboTreeViewItem()
        {
            ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(ComboTreeViewItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceToolTipIsEnabled)));
        }

        public ComboTreeViewItem()
        {

        }



        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }


        public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(typeof(ComboTreeViewItem));




        public object Selection
        {
            get { return (object)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }


        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(object), typeof(CustomTreeViewItem), new PropertyMetadata(SelectionChanged));

        private static void SelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboTreeViewItem && e.NewValue is TreeViewItem treeViewItem)
            {

            }
        }

        public object SelectionBoxItemTemplate
        {
            get { return (object)GetValue(SelectionBoxItemTemplateProperty); }
            set { SetValue(SelectionBoxItemTemplateProperty, value); }
        }


        public static readonly DependencyProperty SelectionBoxItemTemplateProperty = ComboBox.SelectionBoxItemTemplateProperty.AddOwner(typeof(ComboTreeViewItem));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = ComboBox.IsReadOnlyProperty.AddOwner(typeof(ComboTreeViewItem));


        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }


        public static readonly DependencyProperty IsDropDownOpenProperty = ComboBox.IsDropDownOpenProperty.AddOwner(typeof(ComboTreeViewItem), new FrameworkPropertyMetadata(
                                BooleanBoxes.FalseBox,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                new PropertyChangedCallback(OnIsDropDownOpenChanged),
                                new CoerceValueCallback(CoerceIsDropDownOpen)));


        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = base.GetContainerForItemOverride();
            if (item is TreeViewItem treeViewItem)
            {
                treeViewItem.Selected += TreeViewItem_Selected;
            }
            return item;
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                ComboTreeViewItem cb = (ComboTreeViewItem)d;
                if (!cb.IsLoaded)
                {
                    cb.RegisterToOpenOnLoad();
                    return BooleanBoxes.FalseBox;
                }
            }

            return value;
        }

        private static object CoerceToolTipIsEnabled(DependencyObject d, object value)
        {
            ComboTreeViewItem cb = (ComboTreeViewItem)d;
            return cb.IsDropDownOpen ? BooleanBoxes.FalseBox : value;
        }

        private void RegisterToOpenOnLoad()
        {
            Loaded += new RoutedEventHandler(OpenOnLoad);
        }


        private void OpenOnLoad(object sender, RoutedEventArgs e)
        {
            // Open combobox after it has rendered (Loaded is fired before 1st render)
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(delegate (object param)
            {
                CoerceValue(IsDropDownOpenProperty);

                return null;
            }), null);
        }

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboTreeViewItem comboBox = (ComboTreeViewItem)d;

            comboBox.HasMouseEnteredItemsHost = false;

            bool newValue = (bool)e.NewValue;
            bool oldValue = !newValue;

            // Fire accessibility event
            //ComboBoxAutomationPeer peer = UIElementAutomationPeer.FromElement(comboBox) as ComboBoxAutomationPeer;
            //if (peer != null)
            //{
            //    peer.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
            //}

            if (newValue)
            {
                // When the drop down opens, take capture
                Mouse.Capture(comboBox, CaptureMode.SubTree);

                // Select text if editable
                //if (comboBox.IsEditable && comboBox.EditableTextBoxSite != null)
                //    comboBox.EditableTextBoxSite.SelectAll();

                //if (comboBox._clonedElement != null && VisualTreeHelper.GetParent(comboBox._clonedElement) == null)
                //{
                //    comboBox.Dispatcher.BeginInvoke(
                //        DispatcherPriority.Loaded,
                //        (DispatcherOperationCallback)delegate (object arg)
                //        {
                //            ComboBox cb = (ComboBox)arg;
                //            cb.UpdateSelectionBoxItem();

                //            if (cb._clonedElement != null)
                //            {
                //                cb._clonedElement.CoerceValue(FrameworkElement.FlowDirectionProperty);
                //            }

                //            return null;
                //        },
                //        comboBox);
                //}

                // Popup.IsOpen is databound to IsDropDownOpen.  We can't know
                // if IsDropDownOpen will be invalidated before Popup.IsOpen.
                // If we are invalidated first and we try to focus the item, we
                // might succeed (b/c there's a logical link from the item to
                // a PresentationSource).  When the popup finally opens, Focus
                // will be sent to null because Core doesn't know what else to do.
                // So, we must focus the element only after we are sure the popup
                // has opened.  We will queue an operation (at Send priority) to
                // do this work -- this is the soonest we can make this happen.
                comboBox.Dispatcher.BeginInvoke(
                    DispatcherPriority.Send,
                    (DispatcherOperationCallback)delegate (object arg)
                    {
                        ComboTreeViewItem cb = (ComboTreeViewItem)arg;
                        if (cb.IsItemsHostVisible)
                        {
                            cb.NavigateToItem(null, -1, true);
                            //typeof(ItemsControl).GetMethod("NavigateToItem").Invoke(comboBox, new object[] { cb.InternalSelectedInfo, ItemNavigateArgs.Empty, true } /* alwaysAtTopOfViewport */);
                        }
                        return null;
                    },
                    comboBox);

                comboBox.OnDropDownOpened(EventArgs.Empty);
            }
            else
            {
                // If focus is within the subtree, make sure we have the focus so that focus isn't in the disposed hwnd
                //if (comboBox.IsKeyboardFocusWithin)
                //{
                //    if (comboBox.IsEditable)
                //    {
                //        if (comboBox.EditableTextBoxSite != null && !comboBox.EditableTextBoxSite.IsKeyboardFocusWithin)
                //        {
                //            comboBox.Focus();
                //        }
                //    }
                //    else
                //    {
                //        // It's not editable, make sure the combobox has focus
                //        comboBox.Focus();
                //    }
                //}

                // Make sure to clear the highlight when the dropdown closes
                //comboBox.HighlightedInfo = null;

                //if (comboBox.HasCapture)
                //{
                //    Mouse.Capture(null);
                //}

                //// No Popup in the style so fire closed now
                //if (comboBox._dropDownPopup == null)
                //{
                //    comboBox.OnDropDownClosed(EventArgs.Empty);
                //}
            }

            //comboBox.CoerceValue(IsSelectionBoxHighlightedProperty);
            comboBox.CoerceValue(ToolTipService.IsEnabledProperty);

            //comboBox.UpdateVisualState();
        }

        private void NavigateToItem(object item, int elementIndex,/* ItemNavigateArgs itemNavigateArgs,*/ bool alwaysAtTopOfViewport)
        {
            // need to deal with more than 1-D no-wrapping virtualization

            // Perhaps the container isn't generated yet.  In this case we try to shift the view,
            // wait for measure, and then call it again.
            if (item == DependencyProperty.UnsetValue)
            {
                return;
            }

            if (elementIndex == -1)
            {
                elementIndex = Items.IndexOf(item);
                if (elementIndex == -1)
                    return;
            }

            bool isHorizontal = false;
            //if (ItemsHost != null)
            //{
            //    isHorizontal = (ItemsHost.HasLogicalOrientation && ItemsHost.LogicalOrientation == Orientation.Horizontal);
            //}

            FrameworkElement container;
            FocusNavigationDirection direction = isHorizontal ? FocusNavigationDirection.Right : FocusNavigationDirection.Down;
            //MakeVisible(elementIndex, direction, alwaysAtTopOfViewport, out container);

            //FocusItem(NewItemInfo(item, container), itemNavigateArgs);
        }


        //internal void MakeVisible(FrameworkElement container, FocusNavigationDirection direction, bool alwaysAtTopOfViewport)
        //{
        //    if (ScrollHost != null && ItemsHost != null)
        //    {
        //        double oldHorizontalOffset;
        //        double oldVerticalOffset;

        //        FrameworkElement viewportElement = GetViewportElement();

        //        while (container != null && !IsOnCurrentPage(viewportElement, container, direction, false /*fullyVisible*/))
        //        {
        //            oldHorizontalOffset = ScrollHost.HorizontalOffset;
        //            oldVerticalOffset = ScrollHost.VerticalOffset;

        //            container.BringIntoView();

        //            // Wait for layout
        //            ItemsHost.UpdateLayout();

        //            // If offset does not change - exit the loop
        //            if (DoubleUtil.AreClose(oldHorizontalOffset, ScrollHost.HorizontalOffset) &&
        //                DoubleUtil.AreClose(oldVerticalOffset, ScrollHost.VerticalOffset))
        //                break;
        //        }

        //        if (container != null && alwaysAtTopOfViewport)
        //        {
        //            bool isHorizontal = (ItemsHost.HasLogicalOrientation && ItemsHost.LogicalOrientation == Orientation.Horizontal);

        //            FrameworkElement firstElement;
        //            GetFirstItemOnCurrentPage(container, FocusNavigationDirection.Up, out firstElement);
        //            while (firstElement != container)
        //            {
        //                oldHorizontalOffset = ScrollHost.HorizontalOffset;
        //                oldVerticalOffset = ScrollHost.VerticalOffset;

        //                if (isHorizontal)
        //                {
        //                    ScrollHost.LineRight();
        //                }
        //                else
        //                {
        //                    ScrollHost.LineDown();
        //                }

        //                ScrollHost.UpdateLayout();

        //                // If offset does not change - exit the loop
        //                if (DoubleUtil.AreClose(oldHorizontalOffset, ScrollHost.HorizontalOffset) &&
        //                    DoubleUtil.AreClose(oldVerticalOffset, ScrollHost.VerticalOffset))
        //                    break;

        //                GetFirstItemOnCurrentPage(container, FocusNavigationDirection.Up, out firstElement);
        //            }
        //        }
        //    }
        //}

        private void OnPopupClosed(object source, EventArgs e)
        {
            OnDropDownClosed(EventArgs.Empty);
        }


        /// <summary>
        /// Returns true if the ItemsHost is visually connected to the RootVisual of its PresentationSource.
        /// </summary>
        /// <value></value>
        private bool IsItemsHostVisible
        {
            get
            {
                if (typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) is Panel itemsHost)
                {
                    HwndSource source = typeof(PresentationSource).GetMethod("CriticalFromVisual", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, new[] { typeof(DependencyObject) }).Invoke(default, new object[] { itemsHost }) as HwndSource;

                    if (source != null && !source.IsDisposed && source.RootVisual != null)
                    {
                        return source.RootVisual.IsAncestorOf(itemsHost);
                    }
                }

                return false;
            }
        }

        private bool HasMouseEnteredItemsHost
        {
            get; set;
            //get { return _cacheValid[(int)CacheBits.HasMouseEnteredItemsHost]; }
            //set { _cacheValid[(int)CacheBits.HasMouseEnteredItemsHost] = value; }
        }

        //internal TextBox EditableTextBoxSite
        //{
        //    get
        //    {
        //        return _editableTextBoxSite;
        //    }
        //    set
        //    {
        //        _editableTextBoxSite = value;
        //    }
        //}

        protected virtual void OnDropDownOpened(EventArgs e)
        {
            RaiseClrEvent(DropDownOpenedKey, e);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            RaiseClrEvent(DropDownClosedKey, e);
        }
        internal void RaiseClrEvent(EventPrivateKey key, EventArgs args)
        {
            //EventHandlersStore store = EventHandlersStore;
            //if (store != null)
            //{
            //    Delegate handler = store.Get(key);
            //    if (handler != null)
            //    {
            //        ((EventHandler)handler)(this, args);
            //    }
            //}
        }

        /// <summary>
        ///     DropDown Open event
        /// </summary>
        public event EventHandler DropDownOpened
        {
            add {/* EventHandlersStoreAdd(DropDownOpenedKey, value);*/ }
            remove { /*EventHandlersStoreRemove(DropDownOpenedKey, value);*/ }

        }
        private static readonly EventPrivateKey DropDownOpenedKey = new EventPrivateKey();


        public event EventHandler DropDownClosed
        {
            add {/* EventHandlersStoreAdd(DropDownOpenedKey, value);*/ }
            remove { /*EventHandlersStoreRemove(DropDownOpenedKey, value);*/ }

        }
        private static readonly EventPrivateKey DropDownClosedKey = new EventPrivateKey();

    }

    public interface ISelection
    {
        object Selection { get; set; }
    }

    public class dasf : ComboBox
    {

    }
}
