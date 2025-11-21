using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Utility.Reactives;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Trees.Infrastructure
{
    public class TreeTabHelper
    {
        /// <summary>
        /// Routed command which can be used to close a tab.
        /// </summary>
        public static RoutedCommand CloseItemCommand = new RoutedUICommand("Close", "Close", typeof(TreeViewItem));

        public static RoutedCommand AddItemCommand = new RoutedUICommand("Add", "Add", typeof(TreeViewItem));

        #region properties

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItem",
                typeof(object),
                typeof(TreeTabHelper),
                new PropertyMetadata());

        public static object GetSelectedItem(DependencyObject obj)
        {
            return (object)obj.GetValue(SelectedItemProperty);
        }

        public static void SetSelectedItem(DependencyObject obj, object value)
        {
            obj.SetValue(SelectedItemProperty, value);
        }

        public static readonly DependencyProperty EditProperty =
            DependencyProperty.RegisterAttached(
                "Edit",
                typeof(object),
                typeof(TreeTabHelper),
                new PropertyMetadata());

        public static object GetEdit(DependencyObject obj)
        {
            return (object)obj.GetValue(EditProperty);
        }

        public static void SetEdit(DependencyObject obj, object value)
        {
            obj.SetValue(EditProperty, value);
        }



        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.RegisterAttached(
                "EditTemplate",
                typeof(DataTemplate),
                typeof(TreeTabHelper),
                new PropertyMetadata());

        public static DataTemplate GetEditTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(EditTemplateProperty);
        }

        public static void SetEditTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(EditTemplateProperty, value);
        }


        public static readonly DependencyProperty IsLoadedProperty =
            DependencyProperty.RegisterAttached(
                "IsLoaded",
                typeof(bool),
                typeof(TreeTabHelper),
                new PropertyMetadata(onTreeViewLoadedChanged));

        public static bool GetIsLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsLoadedProperty);
        }

        public static void SetIsLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(IsLoadedProperty, value);
        }


        public static readonly DependencyProperty ParentItemCountProperty =
            DependencyProperty.RegisterAttached(
                "ParentItemCount",
                typeof(int?),
                typeof(TreeTabHelper),
                new PropertyMetadata(default, OnItemCountPropertyAttached));

        public static int? GetParentItemCount(DependencyObject obj)
        {
            if (obj is TreeViewItem tvi)
                EnsureHooked(tvi);
            return (int?)obj.GetValue(ParentItemCountProperty);
        }

        public static void SetParentItemCount(DependencyObject obj, int? value)
        {
            obj.SetValue(ParentItemCountProperty, value);
        }

        private static void EnsureHooked(TreeViewItem tvi)
        {
            // Only hook once — use a private flag
            if (tvi.ReadLocalValue(_isHookedProperty) is bool hooked && hooked)
                return;

            tvi.SetValue(_isHookedProperty, true);

            var parent = tvi.FindParent<TreeViewItem>();
            if (parent == null)
                return;
            if (parent.Items is INotifyCollectionChanged incc)
            {
                incc.CollectionChanged += (_, __) => SetParentItemCount(tvi, parent.Items.Count);
            }

            // Initial value
            SetParentItemCount(tvi, parent.Items.Count);
        }

        private static readonly DependencyProperty _isHookedProperty =
            DependencyProperty.RegisterAttached(
                "_isHooked",
                typeof(bool),
                typeof(TreeTabHelper),
                new PropertyMetadata(false));

        private static void OnItemCountPropertyAttached(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeViewItem tvi)
                EnsureHooked(tvi);
        }

        public static readonly DependencyProperty TabOffsetProperty =
            DependencyProperty.RegisterAttached(
                "TabOffset", typeof(Thickness),
                typeof(TreeTabHelper),
                new FrameworkPropertyMetadata(new Thickness(0)));

        public static void SetTabOffset(DependencyObject element, Thickness value)
            => element.SetValue(TabOffsetProperty, value);

        public static Thickness GetTabOffset(DependencyObject element)
            => (Thickness)element.GetValue(TabOffsetProperty);



        public static readonly DependencyProperty TotalTabOffsetProperty =
            DependencyProperty.RegisterAttached(
                "TotalTabOffset", typeof(Thickness),
                typeof(TreeTabHelper),
                new FrameworkPropertyMetadata(new Thickness(0)));

        public static void SetTotalTabOffset(DependencyObject element, Thickness value)
            => element.SetValue(TotalTabOffsetProperty, value);

        public static Thickness GetTotalTabOffset(DependencyObject element)
            => (Thickness)element.GetValue(TotalTabOffsetProperty);


        public static readonly DependencyProperty DoubleProperty =
            DependencyProperty.RegisterAttached(
                "Double", typeof(double),
                typeof(TreeTabHelper),
                new FrameworkPropertyMetadata(1.0));

        public static void SetDouble(DependencyObject element, double value)
            => element.SetValue(DoubleProperty, value);

        public static double GetDouble(DependencyObject element)
            => (double)element.GetValue(DoubleProperty);


        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.RegisterAttached(
                "Depth", typeof(double),
                typeof(TreeTabHelper),
                new FrameworkPropertyMetadata());

        public static void SetDepth(DependencyObject element, double value)
            => element.SetValue(DepthProperty, value);

        public static double GetDepth(DependencyObject element)
            => (double)element.GetValue(DepthProperty);


        public static readonly DependencyProperty RemoveItemCallbackProperty =
            DependencyProperty.RegisterAttached(
                "RemoveItemCallback", typeof(RemoveItemActionCallback),
                typeof(TreeTabHelper),
                new FrameworkPropertyMetadata(default));


        public static void SetRemoveItemCallback(DependencyObject element, object value)
            => element.SetValue(RemoveItemCallbackProperty, value);

        public static RemoveItemActionCallback GetRemoveItemCallback(DependencyObject element)
            => (RemoveItemActionCallback)element.GetValue(RemoveItemCallbackProperty);


        public static readonly DependencyProperty AddItemCallbackProperty =
            DependencyProperty.RegisterAttached(
                "AddItemCallback", typeof(AddItemActionCallback),
                typeof(TreeTabHelper),
                new FrameworkPropertyMetadata(default));


        public static void SetAddItemCallback(DependencyObject element, AddItemActionCallback value)
            => element.SetValue(AddItemCallbackProperty, value);

        public static AddItemActionCallback GetAddItemCallback(DependencyObject element)
            => (AddItemActionCallback)element.GetValue(AddItemCallbackProperty);

        #endregion properties   

        public delegate void RemoveItemActionCallback(ItemActionCallbackArgs<TreeViewItem> args);


        public delegate void AddItemActionCallback(AddItemActionCallbackArgs args);

        public static TreeTabHelper Instance { get; } = new TreeTabHelper();

        private static void onTreeViewLoadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeViewItem _item && e.NewValue is true)
            {
                if (_item.IsLoaded)
                    TabTree_Loaded(_item);
                else
                    _item.Loaded += (_, _) => TabTree_Loaded(_item);
            }

            void TabTree_Loaded(TreeViewItem tabTree)
            {
                double totalWidth = 0;

                if (tabTree.Items is INotifyCollectionChanged incc)
                {
                    incc.NotificationChanges().Subscribe(a =>
                    {
                        switch (a.Action)
                        {
                            case NotifyCollectionChangedAction.Add:

                                int index = a.NewStartingIndex;

                                foreach (var item in a.NewItems)
                                {
                                    if (tabTree.ItemContainerGenerator.ContainerFromIndex(index) is TreeViewItem container)
                                    {
                                        process(container, index);
                                        if (container.IsSelected)
                                        {
                                            ZIndexAnimator.AnimateOnSelected(tabTree, container);
                                        }
                                        container.Selected += (s, _) =>
                                        {
                                            if (container.IsSelected)
                                            {
                                                TreeTabHelper.SetSelectedItem(tabTree, container.DataContext);
                                                ZIndexAnimator.AnimateOnSelected(tabTree, container);
                                            }
                                        };
                                        container.Unloaded += (s, e) =>
                                        {
                                            ZIndexAnimator.AnimateOnRemoval(tabTree, container);
                                            initialise();
                                        };
                                    }
                                    index++;
                                }
                                //if (tabTree.Items.Count > 0 && tabTree.Items.OfType<TreeViewItem>().All(a => a.IsSelected == false) && tabTree.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem _first)
                                //{
                                //    _first.IsSelected = true;
                                //    TreeTabHelper.SetDouble(_first, 1);
                                //}
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                totalWidth = 0;
                                TreeTabHelper.SetTotalTabOffset(tabTree, new Thickness(totalWidth, 0, 0, 0));
                                break;
                        }
                    });
                }

                initialise(true);

                void initialise(bool first = false)
                {
                    totalWidth = 0;
                    for (int i = 0; i < tabTree.Items.Count; i++)
                    {
                        TreeViewItem lastItem;
                        if (tabTree.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item)
                        {
                            lastItem = item;
                            process(item, i);
                            if (first)
                            {
                                item.Selected += (s, _) =>
                                {
                                    if (item.IsSelected)
                                    {
                                        TreeTabHelper.SetSelectedItem(tabTree, item.DataContext);
                                        ZIndexAnimator.AnimateOnSelected(tabTree, item);
                                    }
                                };
                                item.Unloaded += (s, e) =>
                                {
                                    ZIndexAnimator.AnimateOnRemoval(tabTree, item);
                                    initialise();
                                };
                            }
                        }
                    }

                    updateSelected();

                    bool highest = false;
                    for (int i = 0; i < tabTree.Items.Count; i++)
                    {
                        TreeViewItem lastItem;
                        if (tabTree.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item)
                        {
                            if (item.IsSelected)
                            {
                                ZIndexAnimator.AnimateOnSelected(tabTree, item);
                                highest = true;
                            }
                        }
                    }
                    //if (highest == false && tabTree.Items.Count > 0 && tabTree.ItemContainerGenerator.ContainerFromIndex(0) is TreeViewItem _first)
                    //{
                    //    _first.IsSelected = true;
                    //    TreeTabHelper.SetDouble(_first, 1);
                    //}
                }

                void process(TreeViewItem pItem, int index)
                {
                    if (pItem.IsLoaded == false)
                    {
                        pItem.Loaded += (s, e) =>
                        {
                            pItem.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                update(pItem, index);
                            }), DispatcherPriority.Loaded);
                        };
                    }
                    else
                    {
                        update(pItem, index);
                    }
                }

                void updateSelected()
                {
                    for (var i = 0; i < tabTree.ItemContainerGenerator.Items.Count; i++)
                    {
                        if (tabTree.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem tvi)
                        {
                            if (Panel.GetZIndex(tvi) == tabTree.ItemContainerGenerator.Items.Count)
                            {
                                tvi.IsSelected = true;
                                //TreeTabHelper.SetDouble(tvi, 1);
                                break;
                            }
                        }
                    }
                }

                void update(TreeViewItem tvi, int i)
                {
                    TreeTabHelper.SetTabOffset(tvi, new Thickness(totalWidth, 0, 0, 0));
                    totalWidth += VisualTreeExHelper.FindChild<Border>(tvi, "TabHeader").ActualWidth;
                    Panel.SetZIndex(tvi, tabTree.Items.Count - i); // initial z-order                        
                    tvi.SetValue(TreeTabHelper.DepthProperty, 1 - ((tabTree.Items.Count - i) / (tabTree.Items.Count * 1d)));
                    TreeTabHelper.SetTotalTabOffset(tabTree, new Thickness(totalWidth, 0, 0, 0));
                }

                CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(CloseItemCommand, CloseItemClassHandler, CloseItemCanExecuteClassHandler));
                CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(AddItemCommand, AddItemClassHandler, AddItemCanExecuteClassHandler));

                static void CloseItemCanExecuteClassHandler(object sender, CanExecuteRoutedEventArgs e)
                {
                    e.CanExecute = FindOwner(e.Parameter, e.OriginalSource) != default;
                }

                static void AddItemCanExecuteClassHandler(object sender, CanExecuteRoutedEventArgs e)
                {
                    var treeViewItem = e.Parameter as TreeViewItem;
                    treeViewItem ??= VisualTreeExHelper.FindParent<TreeViewItem>(e.OriginalSource as DependencyObject);
                    if (treeViewItem is null)
                        throw new ApplicationException("Valid DragablzItem to close is required.");

                    e.CanExecute = TreeTabHelper.GetAddItemCallback(treeViewItem) is { } callback;
                }

                static void CloseItemClassHandler(object sender, ExecutedRoutedEventArgs e)
                {
                    var owner = FindOwner(e.Parameter, e.OriginalSource);

                    if (owner == default) throw new ApplicationException("Unable to ascertain DragablzItem to close.");

                    CloseItem(owner.Item1, owner.Item2);

                    static void CloseItem(TreeViewItem item, TreeViewItem owner)
                    {
                        if (item == null)
                            throw new ApplicationException("Valid DragablzItem to close is required.");

                        if (owner == null)
                            throw new ApplicationException("Valid DragablzItem container is required.");

                        if (!IsMyItem(owner, item))
                            throw new ApplicationException("DragablzItem container must be an owner of the DragablzItem to close");

                        var cancel = false;
                        if (TreeTabHelper.GetRemoveItemCallback(owner) is { } callback)
                        {
                            var callbackArgs = new ItemActionCallbackArgs<TreeViewItem>(Window.GetWindow(owner), owner, item);
                            callback.Invoke(callbackArgs);
                            cancel = callbackArgs.IsCancelled;
                        }

                        if (!cancel)
                            RemoveItem(owner, item);
                    }

                }
                static void AddItemClassHandler(object sender, ExecutedRoutedEventArgs e)
                {

                    var treeViewItem = e.Parameter as TreeViewItem;
                    treeViewItem ??= VisualTreeExHelper.FindParent<TreeViewItem>(e.OriginalSource as DependencyObject);
                    if (treeViewItem is null)
                        throw new ApplicationException("Valid DragablzItem to close is required.");

                    var cancel = false;
                    if (TreeTabHelper.GetAddItemCallback(treeViewItem) is { } callback)
                    {
                        var callbackArgs = new AddItemActionCallbackArgs(treeViewItem, e.Parameter);
                        callback.Invoke(callbackArgs);
                        cancel = callbackArgs.IsCancelled;

                        if (!cancel)
                        {
                            if (treeViewItem.ItemsSource != null)
                            {
                                if (treeViewItem.ItemsSource is IList collection)
                                    collection.Add(callbackArgs.NewItem);
                                else
                                    throw new Exception("c7hni22 22");
                            }
                            else
                                treeViewItem.Items.Add(callbackArgs.NewItem);
                        }
                    }
                }



                static void RemoveItem(TreeViewItem owner, TreeViewItem dragablzItem)
                {
                    //var item = owner.ItemContainerGenerator.ItemFromContainer(dragablzItem);
                    if (owner.ItemsSource is not null)
                    {
                        if (owner.ItemsSource is IList list)
                            list.Remove(dragablzItem.DataContext);
                    }
                    else
                        owner.Items.Remove(dragablzItem);
                }
            }

            static bool IsMyItem(TreeViewItem owner, TreeViewItem item)
            {
                return Containers<TreeViewItem>(owner).Any(c => c == item);
            }

            static IEnumerable<TContainer> Containers<TContainer>(ItemsControl itemsControl) where TContainer : class
            {
                for (var i = 0; i < itemsControl.ItemContainerGenerator.Items.Count; i++)
                {
                    if (itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is TContainer container)
                        yield return container;
                }
            }

            static (TreeViewItem, TreeViewItem) FindOwner(object eventParameter, object eventOriginalSource)
            {
                if (eventParameter == null)
                    return default;
                var dragablzItem = eventParameter as TreeViewItem;

                var parent = dragablzItem.FindParent<TreeViewItem>();
                return (dragablzItem, parent);
            }
        }

        public class ItemActionCallbackArgs<TOwner> where TOwner : FrameworkElement
        {
            private readonly Window _window;
            private readonly TOwner _owner;
            private readonly TreeViewItem _dragablzItem;

            public ItemActionCallbackArgs(Window window, TOwner owner, TreeViewItem dragablzItem)
            {
                if (window == null) throw new ArgumentNullException("window");
                if (owner == null) throw new ArgumentNullException("owner");
                if (dragablzItem == null) throw new ArgumentNullException("dragablzItem");

                _window = window;
                _owner = owner;
                _dragablzItem = dragablzItem;
            }

            public Window Window
            {
                get { return _window; }
            }

            public TOwner Owner
            {
                get { return _owner; }
            }

            public TreeViewItem DragablzItem
            {
                get { return _dragablzItem; }
            }

            public bool IsCancelled { get; private set; }

            public void Cancel()
            {
                IsCancelled = true;
            }
        }
        public class AddItemActionCallbackArgs
        {

            private readonly TreeViewItem _owner;


            public AddItemActionCallbackArgs(TreeViewItem owner, object newItem)
            {
                _owner = owner;
                NewItem = newItem;
            }

            public TreeViewItem Owner => _owner;


            public object NewItem { get; set; }

            public bool IsCancelled { get; private set; }

            public void Cancel()
            {
                IsCancelled = true;
            }
        }


        public static class ZIndexAnimator
        {

            public static void AnimateOnSelected(TreeViewItem tree, TreeViewItem selectedItem)
            {
                var items = new List<TreeViewItem>();
                for (int i = 0; i < tree.Items.Count; i++)
                {
                    if (tree.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item)
                        items.Add(item);
                }

                int maxZ = items.Count;
                foreach (var item in items)
                {
                    int start = Panel.GetZIndex(item);
                    int target = (item == selectedItem) ? maxZ : Math.Max(0, start - 1);
                    AnimateZStep(item, start, target, maxZ);
                }
            }

            public static void AnimateOnRemoval(TreeViewItem tree, TreeViewItem removedItem)
            {
                var items = new List<TreeViewItem>();

                // Collect remaining items
                for (int i = 0; i < tree.Items.Count; i++)
                {
                    if (tree.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem item)
                    {
                        if (item != removedItem)
                            items.Add(item);
                    }
                }

                int maxZ = items.Count;

                foreach (var item in items)
                {
                    int start = Panel.GetZIndex(item);

                    // If the removed item had a higher Z, shift down
                    int target = start > Panel.GetZIndex(removedItem) ? start - 1 : start;
                    AnimateZStep(item, start, target, maxZ);
                }
            }

            static FrameworkElement _element = new FrameworkElement();


            static void AnimateZStep(TreeViewItem item, int start, int target, int count)
            {
                DoubleAnimation anim = new(start, target, TimeSpan.FromSeconds(0.4))
                {
                    FillBehavior = FillBehavior.Stop
                };

                anim.CurrentTimeInvalidated += (s, _) =>
                {
                    if (s is AnimationClock clock && clock.CurrentProgress.HasValue)
                    {
                        double progressValue = start + (target - start) * clock.CurrentProgress.Value;
                        int steppedZ = (int)Math.Round(progressValue);
                        Panel.SetZIndex(item, steppedZ);
                        item.SetValue(TreeTabHelper.DepthProperty, 1 - (progressValue / count));
                        if (target >= start)
                            item.SetValue(TreeTabHelper.DoubleProperty, clock.CurrentProgress.Value);
                        else
                            item.SetValue(TreeTabHelper.DoubleProperty, 1 - clock.CurrentProgress.Value);
                    }
                };
                // Use dummy element to host animation
                _element.BeginAnimation(TreeTabHelper.DoubleProperty, anim);
            }

        }
    }

    public class ShowCloseButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is int count && count > 1) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class IntensityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double intensity)
            {
                // Scale the intensity to the color value (0 to 255)
                byte colorValue = (byte)(255 - (100 * intensity));
                return Color.FromRgb(colorValue, colorValue, colorValue); // Grayscale
            }

            return Colors.Gray; // Default color if conversion fails
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
