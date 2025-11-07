using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Dragablz;
using Evan.Wpf;
using ReactiveUI;
using Utility.WPF.Abstract;
using Utility.WPF.Attached;
using Utility.WPF.Events;
using Utility.WPF.Helpers;

namespace Utility.WPF.Controls.Dragablz
{
    public class DragablzExItemsControl : DragablzItemsControl, ISelector, ICheckedSelector
    {
        public static readonly DependencyProperty SelectedItemProperty = DependencyHelper.Register<object>();
        public static readonly DependencyProperty SelectedIndexProperty = DependencyHelper.Register<int>();
        public static readonly DependencyProperty CheckedItemsProperty = DependencyHelper.Register<IReadOnlyCollection<object>>();
        public static readonly DependencyProperty UnCheckedItemsProperty = DependencyHelper.Register<IReadOnlyCollection<object>>();
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(DragablzVerticalItemsControl));
        public static readonly RoutedEvent CheckedChangedEvent = EventManager.RegisterRoutedEvent(nameof(CheckedChanged), RoutingStrategy.Bubble, typeof(CheckedChangedEventHandler), typeof(DragablzVerticalItemsControl));

        public DragablzExItemsControl()
        {
        }

        #region properties

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public IEnumerable CheckedItems
        {
            get { return (IEnumerable)GetValue(CheckedItemsProperty); }
            set { SetValue(CheckedItemsProperty, value); }
        }

        public IEnumerable UnCheckedItems
        {
            get { return (IEnumerable)GetValue(UnCheckedItemsProperty); }
            set { SetValue(UnCheckedItemsProperty, value); }
        }

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        public event CheckedChangedEventHandler CheckedChanged
        {
            add { AddHandler(CheckedChangedEvent, value); }
            remove { RemoveHandler(CheckedChangedEvent, value); }
        }

        #endregion properties

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (base.GetContainerForItemOverride() is not DragablzItem item)
                throw new Exception("s444dfsdf");

            item.MouseDown += Child_MouseDown;
            item.GotFocus += Child_GotFocus;
            item.MouseEnter += DragablzExItemsControl_MouseEnter;
            item.PreviewKeyDown += DragablzExItemsControl_PreviewKeyDown;
            item.PreviewKeyUp += DragablzExItemsControl_PreviewKeyDown;

            foreach (var child in item.FindVisualChildren<FrameworkElement>())
            {
                child.MouseDown += Child_MouseDown;
                child.GotFocus += Child_GotFocus;
            }

            item?
                .WhenAny(a => a.IsSelected, (a) => a)
                .Skip(1)
                .ObserveOnDispatcher()
                .SubscribeOnDispatcher()
                .Subscribe(a =>
                {
                    if (a.Value == false)
                    {
                        return;
                    }
                    var items = Items.OfType<object>().Select(a => ItemContainerGenerator.ContainerFromItem(a)).OfType<DragablzItem>().ToArray();
                    var selected = items.Where(a => a.IsSelected).Select(a => a.Content).ToArray();
                    if (selected.Any() == false)
                        return;

                    int i = 0;
                    int index = -1;
                    foreach (var ditem in items)
                    {
                        if (ditem.Content != item.Content && ditem.IsSelected == true)
                            ditem.IsSelected = false;
                        else if (ditem == item)
                        {
                            index = i;
                        }
                        i++;
                    }

                    selected = items.Where(a => a.IsSelected).ToArray();
                    if (selected.Length != 1)
                    {
                        throw new Exception("Only a single item can ever be selected. Make sure 'Equals' method is correctly implemented.");
                    }
                    SelectedItem = selected.Cast<DragablzItem>().Select(a => a.Content).SingleOrDefault();
                    SelectedIndex = index;
                    RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, selected, new[] { a.Sender.Content }));
                });

            Ex.Observable<bool>(a => a == item, a => a.Name == nameof(Ex.IsChecked))
                  //.Skip(1)
                  .ObserveOnDispatcher()
                  .SubscribeOnDispatcher()
                  .Subscribe(a =>
                  {
                      var items = Items.OfType<object>().Select(a => ItemContainerGenerator.ContainerFromItem(a)).Cast<DragablzItem>().ToArray();
                      var @checked = items.Where(a => Ex.GetIsChecked(a)).Select(a => a.Content).ToArray();
                      var @unchecked = items.Where(a => Ex.GetIsChecked(a) == false).Select(a => a.Content).ToArray();

                      RaiseEvent(new CheckedChangedEventArgs(CheckedChangedEvent, this, @checked, @unchecked));
                  });

            return item;
        }

        private void DragablzExItemsControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }

        private void DragablzExItemsControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void Child_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DragablzItem dItem)
            {
                dItem.IsSelected = true;
                return;
            }
            if ((e.OriginalSource as Control)?.FindParent<DragablzItem>() is { } parent)
                parent.IsSelected = true;
        }

        private void Child_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DragablzItem dItem)
            {
                dItem.IsSelected = true;
                return;
            }
            if (sender is DragablzItem parent)
                parent.IsSelected = true;
        }

        private void DragablzExItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is DragablzItem dragablzItem)
                dragablzItem.IsSelected = true;
        }
    }
}