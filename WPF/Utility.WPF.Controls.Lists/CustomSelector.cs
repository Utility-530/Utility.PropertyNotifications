using AnimatedScrollViewer;
using Fasterflect;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Commands;
using Utility.WPF.Controls.Objects;
using Utility.WPF.Helpers;
using Utility.Helpers;

namespace Utility.WPF.Controls.Lists
{
    public class CustomSelector : AnimatedListBox
    {
        public static readonly DependencyProperty CheckedCommandProperty =
            DependencyProperty.Register("CheckedCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata(new Command<object>((a) =>
            {
                if (a is RoutedEventArgs { OriginalSource: ToggleButton { IsChecked: { } isChecked } control })
                {
                    if (control.FindParent<CustomSelector>() is CustomSelector cs)
                        if (string.IsNullOrEmpty(cs.CheckedPropertyName) != isChecked)
                            a.TrySetPropertyValue(cs.CheckedPropertyName, isChecked);
                }

            })));
        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());

        public static readonly DependencyProperty EditProperty =
            DependencyProperty.Register("Edit", typeof(object), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty EditTemplateSelectorProperty =
            DependencyProperty.Register("EditTemplateSelector", typeof(DataTemplateSelector), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(CustomSelector), new PropertyMetadata());

        public static readonly DependencyProperty SwapCommandProperty =
            DependencyProperty.Register("SwapCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());
        //public static readonly DependencyProperty UnCheckedCommandProperty =
        //    DependencyProperty.Register("UnCheckedCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());
        public static readonly DependencyProperty CheckedPropertyNameProperty =
            DependencyProperty.Register("CheckedPropertyName", typeof(string), typeof(CustomSelector), new PropertyMetadata("IsChecked", isCheckedChanged));
        public static readonly DependencyProperty ItemsWidthProperty =
            DependencyProperty.Register("ItemsWidth", typeof(double), typeof(CustomSelector), new PropertyMetadata(150.0));
        public static readonly DependencyProperty ItemsHeightProperty =
            DependencyProperty.Register("ItemsHeight", typeof(double), typeof(CustomSelector), new PropertyMetadata(40.0));



        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(CustomSelector), new PropertyMetadata());



        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(CustomSelector), new PropertyMetadata());




        public bool IsGrid
        {
            get { return (bool)GetValue(IsGridProperty); }
            set { SetValue(IsGridProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsGrid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsGridProperty =
            DependencyProperty.Register("IsGrid", typeof(bool), typeof(CustomSelector), new PropertyMetadata());



        public bool AllowsColumnReorder
        {
            get { return (bool)GetValue(AllowsColumnReorderProperty); }
            set { SetValue(AllowsColumnReorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowsColumnReorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(CustomSelector), new PropertyMetadata(true));


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ItemsSourceProperty)
            {
                if (e.NewValue is not null && this.IsGrid)
                {
                    updateColumns(this.IsGrid, (IEnumerable)e.NewValue);
                }
                else
                {
                    Columns = null;
                }
            }
            if (e.Property == IsGridProperty)
            {
                if (e.NewValue is true && ItemsSource != null)
                {
                    updateColumns(true, ItemsSource);
                }
                else
                {
                    Columns = null;
                }
            }

            void updateColumns(bool b, IEnumerable enumerable)
            {
                var type = enumerable.GetType().GetMethod("get_Item").ReturnType;
                Columns ??= [];
                AutoListViewColumnHelpers.CreateColumns2(this, type).ForEach(c => Columns.Add(c));
                var ex = enumerable.GetEnumerator();
                if (ex.MoveNext())
                    Header = ex.Current;
            }
            base.OnPropertyChanged(e);
        }
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

            this.SelectionChanged += CustomSelector_SelectionChanged;
            base.OnApplyTemplate();
        }

        private void CustomSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
            EditRoutedEventArgs routedEventArgs;

            if (Edit is ObjectWrapper { Object: { } _object } wrapper)
            {
                routedEventArgs = new(isAccepted, _object, FinishEditEvent, this);
            }
            else
            {
                routedEventArgs = new(isAccepted, Edit, FinishEditEvent, this);
            }
            RaiseEvent(routedEventArgs);
            {
                this.GetBindingExpression(CustomSelector.EditProperty)
                                  .UpdateTarget();
            }
        }

        public CustomSelector()
        {
            SwapCommand = new Command<int[]>(
                (indexes) =>
                {
                    if (this.ItemsSource is IList collection)
                        swap(indexes, collection);
                    else if (this.ItemsSource is ICollectionView { SourceCollection: IList sourceCollection } collectionView)
                        swap(indexes, sourceCollection);
                });

            RemoveCommand = new Command<object>((a) =>
            {

                int i = 0;
                if (this.ItemsSource is IList collection)
                    remove(a, collection);
                else if (this.ItemsSource is ICollectionView { SourceCollection: IList sourceCollection } collectionView)
                    remove(a, sourceCollection);
            });

            //CheckedCommand = new Command<object>((a) =>
            //{
            //    if (string.IsNullOrEmpty(CheckedPropertyName) == false)
            //        a.TrySetPropertyValue(CheckedPropertyName, true);

            //});

            //UnCheckedCommand = new Command<object>((a) =>
            //{
            //    if (string.IsNullOrEmpty(CheckedPropertyName) == false)
            //        a.TrySetPropertyValue(CheckedPropertyName, false);
            //});

            void swap(int[] indexes, IList collection)
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

            void remove(object a, IList collection)
            {
                int i = 0;

                foreach (var item in collection)
                {
                    if (item == (a))
                    {
                        collection.RemoveAt(i);
                        return;
                    }
                    i++;
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is ListBoxItem lbi)
            {
                lbi.Loaded += Lbi_Loaded;
            }
            void Lbi_Loaded(object sender, RoutedEventArgs e)
            {
                lbi.Dispatcher.BeginInvoke(() =>
                {
                    if (element.FindChild<CheckBox>() is CheckBox checkBox)
                    {
                        BindingOperations.SetBinding(checkBox, CheckBox.IsCheckedProperty, new Binding { Source = item, Path = new PropertyPath(CheckedPropertyName) });
                    }
                    else
                    {
                        //throw new System.Exception("£333444");
                    }
                }, System.Windows.Threading.DispatcherPriority.Background);

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

        public double ItemsWidth
        {
            get { return (double)GetValue(ItemsWidthProperty); }
            set { SetValue(ItemsWidthProperty, value); }
        }
        public double ItemsHeight
        {
            get { return (double)GetValue(ItemsHeightProperty); }
            set { SetValue(ItemsHeightProperty, value); }
        }

        public ICommand CheckedCommand
        {
            get { return (ICommand)GetValue(CheckedCommandProperty); }
            set { SetValue(CheckedCommandProperty, value); }
        }

        //public ICommand UnCheckedCommand
        //{
        //    get { return (ICommand)GetValue(UnCheckedCommandProperty); }
        //    set { SetValue(UnCheckedCommandProperty, value); }
        //}

        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public object Edit
        {
            get { return (object)GetValue(EditProperty); }
            set { SetValue(EditProperty, value); }
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
