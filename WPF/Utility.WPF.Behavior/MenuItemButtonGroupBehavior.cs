using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Reactive;
using System.Windows.Controls.Primitives;

namespace Utility.WPF.Behavior
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/3652688/mutually-exclusive-checkable-menu-items/11497189#11497189"></a>
    /// </summary>
    public class MenuItemButtonGroupBehavior : Behavior<MenuItem>
    {
        private ReplaySubject<MenuItem[]> replaySubject = new();

        public static readonly DependencyProperty SelectedItemProperty =
        Selector.SelectedItemProperty.AddOwner(typeof(MenuItemButtonGroupBehavior), new PropertyMetadata(null, Change));

        private static void Change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            replaySubject
                        .Subscribe(a =>
                        {
                            a.ToList()
                            .ForEach(item =>
                            {
                                item.IsCheckable = true;
                                item.Click += OnClick;
                            });
                        });

            CheckableSubMenuItems(AssociatedObject).Subscribe(replaySubject);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            replaySubject
                .Subscribe(a =>
                {
                    a.ToList()
                    .ForEach(item => item.Click -= OnClick);
                });
        }


        private void OnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            if (sender is not MenuItem menuItem)
            {
                return;
            }

            replaySubject
                         .Subscribe(a =>
                        {
                            //.Where(item => item != menuItem)
                            a
                            .ToList()
                            .ForEach(item =>
                            {

                                if (item.IsChecked = item == menuItem)
                                    SelectedItem = item.DataContext;
                            });
                        });
        }

        private static IObservable<MenuItem[]> CheckableSubMenuItems(ItemsControl menuItem)
        {
            return menuItem
                .WhenAnyValue(a => a.Items)
                .CombineLatest(menuItem.ItemContainerGenerator.ContainersGeneratedChanges(), (a, b) => a)
                .Select(items =>
                {
                    var ret = items
                    .Cast<object>()
                    .Select((a) => a is MenuItem ? a : menuItem.ItemContainerGenerator.ContainerFromItem(a))
                        .OfType<MenuItem>()
                        .ToArray();
                    return ret;
                });
        }

    }
}
