using AnimatedScrollViewer;
using Fasterflect;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Commands;
using Utility.WPF.Helpers;

namespace Utility.WPF.Demo.SandBox
{
    public delegate void FinishEditRoutedEventHandler(object sender, NewObjectRoutedEventArgs e);

    public class NewObjectRoutedEventArgs : RoutedEventArgs
    {
        public NewObjectRoutedEventArgs(bool isAccepted, object newObject, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            NewObject = newObject;
            IsAccepted = isAccepted;
        }

        public object NewObject { get; }
        public bool IsAccepted { get; }
    }

    public class CustomSelector : AnimatedListBox
    {
        public static readonly DependencyProperty CheckedCommandProperty =
            DependencyProperty.Register("CheckedCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty NewObjectProperty =
            DependencyProperty.Register("NewObject", typeof(object), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty SwapCommandProperty =
            DependencyProperty.Register("SwapCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty EditTemplateSelectorProperty =
            DependencyProperty.Register("EditTemplateSelector", typeof(DataTemplateSelector), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty UnCheckedCommandProperty =
            DependencyProperty.Register("UnCheckedCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty CheckedPropertyNameProperty =
            DependencyProperty.Register("CheckedPropertyName", typeof(string), typeof(CustomSelector), new PropertyMetadata("IsChecked", isCheckedChanged));

        private static void isCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomSelector sel && e.NewValue is string str)
            {

            }
        }

        public static readonly RoutedEvent FinishEditEvent = EventManager.RegisterRoutedEvent(
         name: "FinishEdit",
         routingStrategy: RoutingStrategy.Bubble,
         handlerType: typeof(FinishEditRoutedEventHandler),
         ownerType: typeof(CustomSelector));

        static CustomSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSelector), new FrameworkPropertyMetadata(typeof(CustomSelector)));
        }

        public override void OnApplyTemplate()
        {
            if (this.GetTemplateChild("Accept_Button") is Button button)
                button.Click += AcceptComboTreeViewItem_Click;
            if (this.GetTemplateChild("Decline_Button") is Button _button)
                _button.Click += DeclineComboTreeViewItem_Click;
            if (Style?.Resources["EditTemplate"] is DataTemplate dataTemplate)
                EditTemplate ??= dataTemplate;

            base.OnApplyTemplate();
        }

        private void DeclineComboTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomRoutedEvent(false);
        }

        public void AcceptComboTreeViewItem_Click(object sender, RoutedEventArgs e)
        {
            RaiseCustomRoutedEvent(true);
        }

        void RaiseCustomRoutedEvent(bool isAccepted)
        {
            NewObjectRoutedEventArgs routedEventArgs = new(isAccepted, NewObject, FinishEditEvent, this);
            RaiseEvent(routedEventArgs);
            {
                this.GetBindingExpression(CustomSelector.NewObjectProperty)
                                  .UpdateTarget();
            }
        }

        public CustomSelector()
        {
            SwapCommand = new Command<int[]>(
                (indexes) =>
                {
                    if (this.ItemsSource is IList collection)
                    {
                        int fromS = indexes[0];
                        int to = indexes[1];
                        var elementSource = collection[to];
                        var dragged = collection[fromS];
                        //if (fromS > to)
                        //{
                        //    collection.Remove(dragged);
                        //    collection.Insert(to, dragged);
                        //}
                        //else
                        //{
                        //collection.Remove(dragged);
                        collection[to] = dragged;
                        collection[fromS] = elementSource;
                        //}
                        //}
                    }
                });

            RemoveCommand = new Command<object>((a) =>
            {

                int i = 0;
                if (this.ItemsSource is IList collection)
                    foreach (var item in collection)
                    {
                        if (item == (a))
                        {
                            collection.RemoveAt(i);
                            return;
                        }
                        i++;
                    }
            });

            CheckedCommand = new Command<object>((a) =>
            {

                if (string.IsNullOrEmpty(CheckedPropertyName) == false)
                    a.TrySetPropertyValue(CheckedPropertyName, true);

            });

            UnCheckedCommand = new Command<object>((a) =>
            {
                if (string.IsNullOrEmpty(CheckedPropertyName) == false)
                    a.TrySetPropertyValue(CheckedPropertyName, false);
            });
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is ListBoxItem lbi)
            {
                lbi.Loaded += Lbi_Loaded;
            }
            void Lbi_Loaded(object sender, RoutedEventArgs e)
            {
                BindingOperations.SetBinding(element.FindChild<CheckBox>(), CheckBox.IsCheckedProperty, new Binding { Source = item, Path = new PropertyPath(CheckedPropertyName) });

            }
            base.PrepareContainerForItemOverride(element, item);
        }


        #region events

        public event FinishEditRoutedEventHandler FinishEdit
        {
            add { AddHandler(FinishEditEvent, value); }
            remove { RemoveHandler(FinishEditEvent, value); }
        }

        #endregion events
        #region properties

        public ICommand CheckedCommand
        {
            get { return (ICommand)GetValue(CheckedCommandProperty); }
            set { SetValue(CheckedCommandProperty, value); }
        }

        public ICommand UnCheckedCommand
        {
            get { return (ICommand)GetValue(UnCheckedCommandProperty); }
            set { SetValue(UnCheckedCommandProperty, value); }
        }

        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public object NewObject
        {
            get { return (object)GetValue(NewObjectProperty); }
            set { SetValue(NewObjectProperty, value); }
        }

        public DataTemplate EditTemplate
        {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
        }

        public DataTemplateSelector EditTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(EditTemplateSelectorProperty); }
            set { SetValue(EditTemplateSelectorProperty, value); }
        }

        public ICommand SwapCommand
        {
            get { return (ICommand)GetValue(SwapCommandProperty); }
            set { SetValue(SwapCommandProperty, value); }
        }

        public string CheckedPropertyName
        {
            get { return (string)GetValue(CheckedPropertyNameProperty); }
            set { SetValue(CheckedPropertyNameProperty, value); }
        }
        #endregion properties

    }
}
