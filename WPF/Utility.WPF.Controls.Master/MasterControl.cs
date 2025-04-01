using Endless;
using Evan.Wpf;
using Microsoft.Xaml.Behaviors;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Utility.WPF.Abstract;
using Utility.WPF.Controls.Base;
using Utility.Enums;
using Visibility = System.Windows.Visibility;

namespace Utility.WPF.Controls.Master
{
    [Flags]
    public enum RemoveOrder
    {
        None,
        Selected = 1,
        Last = 2,
    }

    public class MasterControl : SelectorAndContentControl, IChange
    {
        [Flags]
        public enum ButtonType
        {
            None = 0, Add = 1, Remove = 2, MoveUp = 4, MoveDown = 8, Enable = 16, All = Add | Remove | MoveUp | MoveDown | Enable,
            Disable = 32
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyHelper.Register<IEnumerator>();

        /// <summary>
        /// Warning!!! Setting this property can mean items get removed via the view.
        /// </summary>
        public static readonly DependencyProperty RemoveOrderProperty = DependencyHelper.Register<RemoveOrder>(new PropertyMetadata(RemoveOrder.Selected));

        public static readonly DependencyProperty ButtonTypesProperty = DependencyHelper.Register<ButtonType>(new PropertyMetadata(ButtonType.All));
        public static readonly RoutedEvent ChangeEvent = EventManager.RegisterRoutedEvent(nameof(Change), RoutingStrategy.Bubble, typeof(CollectionChangedEventHandler), typeof(MasterControl));
        private Button buttonEnable;
        private Button buttonDisable;

        static MasterControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(typeof(MasterControl)));
            DataContextProperty.OverrideMetadata(typeof(MasterControl), new FrameworkPropertyMetadata(null, Changed));
        }

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public MasterControl()
        {
            this.WhenAnyValue(a => a.DataContext)
                .Subscribe(a =>
                {
                });
            this.WhenAnyValue(a => a.ItemsSource)
                .Subscribe(a =>
                {
                });
        }

        #region properties

        public IEnumerator CommandParameter
        {
            get => (IEnumerator)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public RemoveOrder RemoveOrder
        {
            get => (RemoveOrder)GetValue(RemoveOrderProperty);
            set => SetValue(RemoveOrderProperty, value);
        }

        public ButtonType ButtonTypes
        {
            get => (ButtonType)GetValue(ButtonTypesProperty);
            set => SetValue(ButtonTypesProperty, value);
        }

        public event CollectionChangedEventHandler Change
        {
            add => AddHandler(ChangeEvent, value);
            remove => RemoveHandler(ChangeEvent, value);
        }

        #endregion properties

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Header is not Panel header)
            {
                header = (GetTemplateChild("PART_HeaderPresenter") as ContentPresenter)?.Content as Panel;
            }

            if (header != null)
            {
                var buttons = header.Children.OfType<Button>().ToArray();
                var buttonAdd = buttons.Single(a => a.Name == "ButtonPlus");
                var buttonRemove = buttons.Single(a => a.Name == "ButtonMinus");
                var buttonMoveUp = buttons.Single(a => a.Name == "ButtonMoveUp");
                var buttonMoveDown = buttons.Single(a => a.Name == "ButtonMoveDown");
                buttonEnable = buttons.Single(a => a.Name == "ButtonEnable");
                buttonDisable = buttons.Single(a => a.Name == "ButtonDisable");

                this.WhenAnyValue(a => a.ButtonTypes).Subscribe(buttonType =>
                 {
                     buttonAdd.Visibility = buttonType.HasFlag(ButtonType.Add) ? Visibility.Visible : Visibility.Collapsed;
                     buttonRemove.Visibility = buttonType.HasFlag(ButtonType.Remove) ? Visibility.Visible : Visibility.Collapsed;
                     buttonMoveUp.Visibility = buttonType.HasFlag(ButtonType.MoveUp) ? Visibility.Visible : Visibility.Collapsed;
                     buttonMoveDown.Visibility = buttonType.HasFlag(ButtonType.MoveDown) ? Visibility.Visible : Visibility.Collapsed;
                     buttonEnable.Visibility = buttonType.HasFlag(ButtonType.Enable) ? Visibility.Visible : Visibility.Collapsed;
                     buttonDisable.Visibility = buttonType.HasFlag(ButtonType.Disable) ? Visibility.Visible : Visibility.Collapsed;
                 });

                buttonAdd.Click += (_, _) => ExecuteAdd();
                buttonRemove.Click += (_, _) => ExecuteRemove();
                buttonMoveUp.Click += (_, _) => ExecuteMoveUp();
                buttonMoveDown.Click += (_, _) => ExecuteMoveDown();
                buttonEnable.Click += (_, _) => ExecuteEnable(true);
                buttonDisable.Click += (_, _) => ExecuteEnable(false);
            }
            ExecuteEnable(true);
        }

        protected virtual void ExecuteAdd()
        {
            //if (CommandParameter?.MoveNext() == true)
            //{
            //    if (Content is DragablzItemsControl itemsControl)
            //        try
            //        {
            //            itemsControl.AddToSource(CommandParameter.Current, AddLocationHint.Last);

            //        }
            //        catch (Exception ex)
            //        {
            //        }
            //}
            //else
            //{
            //}
            RaiseEvent(new CollectionItemEventArgs(EventType.Add, SelectedItem, SelectedIndex, ChangeEvent));
        }

        protected virtual void ExecuteRemove()
        {
            RaiseEvent(new CollectionItemChangedEventArgs(ItemsSource, new[] { SelectedItem }, EventType.Remove, SelectedItem, SelectedIndex, ChangeEvent));

            if (ItemsSource != null)
            {
                if (ItemsSource is IList { IsReadOnly: false, IsFixedSize: false, Count: > 0 } collection)
                {
                    if (RemoveOrder == RemoveOrder.Selected && SelectedIndex > -1)
                    {
                        var item = ItemsSource.Cast<object>().ElementAt(SelectedIndex);
                        collection.RemoveAt(SelectedIndex);
                        RaiseEvent(new CollectionItemChangedEventArgs(collection, new[] { item }, EventType.Removed, item, SelectedIndex, ChangeEvent));
                    }

                    if (RemoveOrder == RemoveOrder.Last)
                    {
                        var item = ItemsSource.Cast<object>().Last();
                        collection.RemoveAt(collection.Count - 1);
                        RaiseEvent(new CollectionItemChangedEventArgs(collection, new[] { item }, EventType.Removed, item, SelectedIndex, ChangeEvent));
                    }
                }
            }
            else
            {
            }
        }

        protected virtual void ExecuteMoveUp()
        {
            var list = IndexedObjects();
            List<IndexedObject> changes = new();
            var index = ItemsSource.OfType<object>().IndexOf(SelectedItem);
            if (index != 0)
            {
                list[index - 1].Index += 1;
                list[index].Index -= 1;
                changes.Add(list[index]);
            }
            RaiseEvent(new MovementEventArgs(list, changes, EventType.MoveUp, SelectedItem, SelectedIndex, ChangeEvent));
        }

        protected virtual void ExecuteMoveDown()
        {
            var list = IndexedObjects();
            List<IndexedObject> changes = new();
            if (SelectedItem != null) { }
            var index = ItemsSource.OfType<object>().IndexOf(SelectedItem);
            if (index != ItemsSource.OfType<object>().Count() - 1)
            {
                list[index + 1].Index -= 1;
                list[index].Index += 1;
                changes.Add(list[index]);
            }
            RaiseEvent(new MovementEventArgs(list, changes, EventType.MoveUp, SelectedItem, SelectedIndex, ChangeEvent));
        }

        protected virtual void ExecuteEnable(bool v)
        {
            //if (contentPresenter == null)
            //    throw new Exception("sfd3 vdfgdf");
            //buttonEnable.IsEnabled = !(buttonDisable.IsEnabled = contentPresenter.IsEnabled = v);

            RaiseEvent(new CollectionItemChangedEventArgs(ItemsSource, default, v ? EventType.Enable : EventType.Disable, default, default, ChangeEvent));
        }

        protected virtual List<IndexedObject> IndexedObjects()
        {
            List<IndexedObject> list = new();
            foreach (var item in ItemsSource.OfType<object>())
            {
                var oldIndex = ItemsSource.OfType<object>().IndexOf(item);
                list.Add(new(item, oldIndex, oldIndex));
            }

            return list;
        }
    }

    public class DisableBehavior : Behavior<MasterControl>
    {
        private readonly IDisposable? compositeDisposable;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.RemoveOrder = RemoveOrder.None;
            AssociatedObject.Change += AssociatedObject_Change;
        }

        private void AssociatedObject_Change(object sender, CollectionEventArgs e)
        {
            //if (e.EventType == EventType.Remove)
            //{
            //    var element = AssociatedObject.ItemsControl.ItemContainerGenerator.ContainerFromItem(e.Item);
            //    if (element is UIElement uiElement)
            //    {
            //        Storyboard myStoryboard = OpacityAnimation(uiElement, 0.4);
            //        myStoryboard.Begin();
            //    }
            //}

            //if (e.EventType == EventType.Add)
            //{
            //    var element = AssociatedObject.ItemsControl.ItemContainerGenerator.ContainerFromItem(e.Item);
            //    if (element is UIElement uiElement)
            //    {
            //        Storyboard myStoryboard = OpacityAnimation(uiElement, 1);
            //        myStoryboard.Begin();
            //    }
            //}

            static Storyboard OpacityAnimation(UIElement uiElement, double opacity)
            {
                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    To = opacity,
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 300))
                };
                Storyboard.SetTarget(opacityAnimation, uiElement);
                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(UIElement.OpacityProperty));
                Storyboard myStoryboard = new Storyboard();
                myStoryboard.Children.Add(opacityAnimation);
                return myStoryboard;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Change -= AssociatedObject_Change;
            base.OnDetaching();
            compositeDisposable?.Dispose();
        }
    }
}