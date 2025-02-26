using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Utility.WPF.Panels;

namespace Utility.WPF.Controls.Trees
{


    static class CustomHelper
    {
        public static T Load<T>(Uri uri, string key) where T : class
        {
            ResourceDictionary res;
            if (Application.Current.Resources.Contains(uri) == false)
            {
                Application.Current.Resources.Add(uri, res = Application.LoadComponent(uri) as ResourceDictionary);
            }
            else
            {
                res = Application.Current.Resources[uri] as ResourceDictionary;
            }

            return res[key] as T;
        }
    }

    public class ComboTreeView : TreeView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem() { ItemContainerStyleSelector = ItemContainerStyleSelector, ItemContainerStyle = CustomHelper.Load<Style>(new Uri($"/{typeof(ComboTreeView).Assembly.GetName().Name};component/Themes/ComboTreeViewItem.xaml", UriKind.RelativeOrAbsolute), "ComboTreeViewItem") };
        }
    }

    public class ComboTreeViewItem : CustomTreeViewItem
    {
        public ComboTreeViewItem()
        {
            Style ??= CustomHelper.Load<Style>(new Uri($"/{typeof(ComboTreeView).Assembly.GetName().Name};component/Themes/ComboTreeViewItem.xaml", UriKind.RelativeOrAbsolute), "ComboTreeViewItem");
            ItemContainerStyle ??= CustomHelper.Load<Style>(new Uri($"/{typeof(ComboTreeView).Assembly.GetName().Name};component/Themes/CustomTreeViewItem.xaml", UriKind.RelativeOrAbsolute), "CustomTreeViewItem");

        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new CustomTreeViewItem() { Style = ItemContainerStyle /*ItemContainerStyleSelector = ItemContainerStyleSelector, ItemContainerStyle = ItemContainerStyle*/ };
            item.Selected += TreeViewItem_Selected;
            return item;
        }

    }

    public partial class CustomTreeViewItem : ISelection
    {
        public static readonly RoutedEvent FinishEditEvent = EventManager.RegisterRoutedEvent(
            name: "FinishEdit",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(FinishEditRoutedEventHandler),
            ownerType: typeof(CustomTreeViewItem));

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
            name: "SelectionChanged",
        routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(System.Windows.Controls.SelectionChangedEventHandler),
            ownerType: typeof(CustomTreeViewItem));

        public static readonly DependencyProperty IsDropDownOpenProperty = ComboBox.IsDropDownOpenProperty.AddOwner(typeof(CustomTreeViewItem), new FrameworkPropertyMetadata(
                BooleanBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                new PropertyChangedCallback(OnIsDropDownOpenChanged),
                new CoerceValueCallback(CoerceIsDropDownOpen)));

        public static readonly DependencyProperty NewObjectProperty = DependencyProperty.Register("NewObject", typeof(object), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty MaxDropDownHeightProperty = ComboBox.MaxDropDownHeightProperty.AddOwner(typeof(CustomTreeViewItem));
        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(object), typeof(CustomTreeViewItem), new PropertyMetadata(_selectionChanged));
        public static readonly DependencyProperty SelectionBoxTemplateSelectorProperty = DependencyProperty.Register("SelectionBoxTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTreeViewItem), new PropertyMetadata());
        public static readonly DependencyProperty SelectionBoxItemTemplateProperty = ComboBox.SelectionBoxItemTemplateProperty.AddOwner(typeof(CustomTreeViewItem));
        public static readonly DependencyProperty IsReadOnlyProperty = ComboBox.IsReadOnlyProperty.AddOwner(typeof(CustomTreeViewItem));


        static CustomTreeViewItem()
        {
            ToolTipService.IsEnabledProperty.OverrideMetadata(typeof(CustomTreeViewItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CoerceToolTipIsEnabled)));
            EventManager.RegisterClassHandler(typeof(ComboBox), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(TreeViewItem), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTreeViewItem), new FrameworkPropertyMetadata(typeof(CustomTreeViewItem)));
        }

        public CustomTreeViewItem()
        {
            initialiseCommands();
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            //TreeView tv = tvi.ParentTreeView;
            //if (tv != null)
            //{
            //    tv.HandleMouseButtonDown();
            //}
        }

        public override void OnApplyTemplate()
        {
            if (this.GetTemplateChild("Accept_Button") is Button button)
                button.Click += AcceptComboTreeViewItem_Click;
            if (this.GetTemplateChild("Decline_Button") is Button _button)
                _button.Click += DeclineComboTreeViewItem_Click;
            if (Style?.Resources["EditTemplate"] is DataTemplate dataTemplate)
                EditTemplate ??= dataTemplate;
            this.onApplyAnimatedTemplate();
            base.OnApplyTemplate();
        }

        private void DeclineComboTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomRoutedEvent(false);
        }

        public void AcceptComboTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomRoutedEvent(true);


            //this.Items.Refresh();
            //this.UpdateLayout();
        }


        void RaiseCustomRoutedEvent(bool isAccepted)
        {
            NewObjectRoutedEventArgs routedEventArgs = new(isAccepted, NewObject, FinishEditEvent, this);
            RaiseEvent(routedEventArgs);
            {
                this.GetBindingExpression(CustomTreeViewItem.NewObjectProperty)?
                                  .UpdateTarget();
            }

            FinishEditCommand?.Execute(routedEventArgs);
        }




        #region events

        public event FinishEditRoutedEventHandler FinishEdit
        {
            add { AddHandler(FinishEditEvent, value); }
            remove { RemoveHandler(FinishEditEvent, value); }
        }

        public event System.Windows.Controls.SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }


        #endregion events

        #region properties

        public object NewObject
        {
            get { return (object)GetValue(NewObjectProperty); }
            set { SetValue(NewObjectProperty, value); }
        }

        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }


        public object Selection
        {
            get { return (object)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
        }

        public DataTemplate SelectionBoxItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectionBoxItemTemplateProperty); }
            set { SetValue(SelectionBoxItemTemplateProperty, value); }
        }





        public DataTemplateSelector SelectionBoxTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectionBoxTemplateSelectorProperty); }
            set { SetValue(SelectionBoxTemplateSelectorProperty, value); }
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

        #endregion properties


        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = new CustomTreeViewItem() { ItemContainerStyleSelector = ItemContainerStyleSelector, ItemContainerStyle = ItemContainerStyle };
            item.Selected += TreeViewItem_Selected;
            return item;
        }

        private static void _selectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomTreeViewItem combo)
            {
                combo.SetValue(IsDropDownOpenProperty, BooleanBoxes.FalseBox);
                combo.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, new[] { e.OldValue }, new[] { e.NewValue }));
            }
        }
        protected void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            if ((bool)value)
            {
                CustomTreeViewItem cb = (CustomTreeViewItem)d;
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
            CustomTreeViewItem cb = (CustomTreeViewItem)d;
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
            CustomTreeViewItem comboBox = (CustomTreeViewItem)d;

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
                        CustomTreeViewItem cb = (CustomTreeViewItem)arg;
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



        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (sender is not CustomTreeViewItem comboBox)
            {
                return;
            }
            //ComboTreeViewItem comboBox = (ComboTreeViewItem)sender;

            // ISSUE (jevansa) -- task 22022:
            //        We need a general mechanism to do this, or at the very least we should
            //        share it amongst the controls which need it (Popup, MenuBase, ComboBox).
            if (Mouse.Captured != comboBox)
            {
                if (e.OriginalSource == comboBox)
                {
                    // If capture is null or it's not below the combobox, close.
                    // More workaround for task 22022 -- check if it's a descendant (following Logical links too)
                    if (Mouse.Captured == null || !(bool)typeof(MenuBase).GetMethod("IsDescendant", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(default, new object[] { comboBox, Mouse.Captured as DependencyObject }))
                    {
                        comboBox.Close();
                    }
                }
                else
                {
                    if ((bool)typeof(MenuBase).GetMethod("IsDescendant", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(default, new object[] { comboBox, e.OriginalSource as DependencyObject }))
                    {
                        // Take capture if one of our children gave up capture (by closing their drop down)
                        if (comboBox.IsDropDownOpen && Mouse.Captured == null && MS.Win32.SafeNativeMethods.GetCapture() == IntPtr.Zero)
                        {
                            Mouse.Capture(comboBox, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        comboBox.Close();
                    }
                }
            }
        }

        private void Close()
        {
            if (IsDropDownOpen)
            {
                this.SetValue(IsDropDownOpenProperty, false);
            }
        }


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

        public void Select(object obj)
        {
            this.Selection = obj;
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
        void Select(object obj);
    }

}

namespace MS.Win32
{
    public class SafeNativeMethods
    {
        internal static class ExternDll
        {
            public const string Activeds = "activeds.dll";
            public const string Advapi32 = "advapi32.dll";
            public const string Comctl32 = "comctl32.dll";
            public const string Comdlg32 = "comdlg32.dll";
            public const string DwmAPI = "dwmapi.dll";
            public const string Gdi32 = "gdi32.dll";
            public const string Gdiplus = "gdiplus.dll";
            public const string Hhctrl = "hhctrl.ocx";
            public const string Imm32 = "imm32.dll";
            public const string Kernel32 = "kernel32.dll";
            public const string Loadperf = "Loadperf.dll";
            public const string Mqrt = "mqrt.dll";
            public const string Mscoree = "mscoree.dll";
            public const string MsDrm = "msdrm.dll";
            public const string Mshwgst = "mshwgst.dll";
            public const string Msi = "msi.dll";
            public const string NaturalLanguage6 = "naturallanguage6.dll";
            public const string Ntdll = "ntdll.dll";
            public const string Ole32 = "ole32.dll";
            public const string Oleacc = "oleacc.dll";
            public const string Oleaut32 = "oleaut32.dll";
            public const string Olepro32 = "olepro32.dll";
            public const string Penimc = "PenIMC_cor3.dll";
            public const string PresentationCore = "PresentationCore.dll";
            public const string PresentationFramework = "PresentationFramework.dll";
            public const string PresentationHostDll = "PresentationHost_cor3.dll";
            public const string PresentationNativeDll = "PresentationNative_cor3.dll";
            public const string Psapi = "psapi.dll";
            public const string Shcore = "shcore.dll";
            public const string Shell32 = "shell32.dll";
            public const string Shfolder = "shfolder.dll";
            public const string Urlmon = "urlmon.dll";
            public const string User32 = "user32.dll";
            public const string Uxtheme = "uxtheme.dll";
            public const string Version = "version.dll";
            public const string Vsassert = "vsassert.dll";
            public const string WindowsBase = "windowsbase.dll";
            public const string Wininet = "wininet.dll";
            public const string Winmm = "winmm.dll";
            public const string Winspool = "winspool.drv";
            public const string Wldp = "wldp.dll";
            public const string WpfGfx = "WpfGfx_cor3.dll";
            public const string WtsApi32 = "wtsapi32.dll";
        }

        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetCapture();
    }
}
