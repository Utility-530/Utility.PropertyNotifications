using AnimatedScrollViewer;
using Fasterflect;
using Itenso.Windows.Controls.ListViewLayout;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Controls.Objects;
using Utility.WPF.Helpers;

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

        public static readonly DependencyProperty CheckedPropertyNameProperty =
            DependencyProperty.Register("CheckedPropertyName", typeof(string), typeof(CustomSelector), new PropertyMetadata("IsChecked"));

        public static readonly DependencyProperty ItemsWidthProperty =
            DependencyProperty.Register("ItemsWidth", typeof(double), typeof(CustomSelector), new PropertyMetadata(150.0));

        public static readonly DependencyProperty ItemsHeightProperty =
            DependencyProperty.Register("ItemsHeight", typeof(double), typeof(CustomSelector), new PropertyMetadata(40.0));

        public static readonly DependencyProperty DuplicateCommandProperty =
            DependencyProperty.Register("DuplicateCommand", typeof(ICommand), typeof(CustomSelector), new PropertyMetadata());

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(CustomSelector), new PropertyMetadata());

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(CustomSelector), new PropertyMetadata());

        public static readonly DependencyProperty IsGridProperty =
            DependencyProperty.Register("IsGrid", typeof(bool), typeof(CustomSelector), new PropertyMetadata());

        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(CustomSelector), new PropertyMetadata(true));

        public ICommand DuplicateCommand
        {
            get { return (ICommand)GetValue(DuplicateCommandProperty); }
            set { SetValue(DuplicateCommandProperty, value); }
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public bool IsGrid
        {
            get { return (bool)GetValue(IsGridProperty); }
            set { SetValue(IsGridProperty, value); }
        }

        public bool AllowsColumnReorder
        {
            get { return (bool)GetValue(AllowsColumnReorderProperty); }
            set { SetValue(AllowsColumnReorderProperty, value); }
        }


        static CustomSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSelector), new FrameworkPropertyMetadata(typeof(CustomSelector)));
        }

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
            else if (e.Property == IsGridProperty)
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
            else if (e.Property == EditProperty)
            {

            }

            void updateColumns(bool b, IEnumerable enumerable)
            {
                Columns ??= [];
                if (enumerable is ListCollectionView view)
                {
                    var type = view.CurrentItem.GetType();

                    if (type != null)
                        AutoListViewColumnHelpers.CreateColumns2(this, type).ForEach(c => Columns.Add(c));
                    else
                        throw new System.Exception("Sfd 3333f cvxs");
                }
                else
                {
                    var type = enumerable.GetType().GetMethod("get_Item")?.ReturnType;

                    if (type != null)
                        AutoListViewColumnHelpers.CreateColumns2(this, type).ForEach(c => Columns.Add(c));
                    else
                        throw new System.Exception("Sfd 3333333f cvxs");
                }

                //var manager = new ListViewLayoutManager(this, Columns);
                var ex = enumerable.GetEnumerator();
                if (ex.MoveNext())
                    Header = ex.Current;
            }
            base.OnPropertyChanged(e);
        }


        public static readonly RoutedEvent FinishEditEvent = EventManager.RegisterRoutedEvent(
         name: "FinishEdit",
         routingStrategy: RoutingStrategy.Bubble,
         handlerType: typeof(FinishEditRoutedEventHandler),
         ownerType: typeof(CustomSelector));


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

            SwapCommand ??= new Command<int[]>(
             (indexes) =>
             {
                 if (this.ItemsSource is IList collection)
                     swap(indexes, collection);
                 else if (this.ItemsSource is ICollectionView { SourceCollection: IList sourceCollection } collectionView)
                     swap(indexes, sourceCollection);
             });

            RemoveCommand ??= new Command<object>((a) =>
            {

                int i = 0;
                if (this.ItemsSource is IList collection)
                    remove(a, collection);
                else if (this.ItemsSource is ICollectionView { SourceCollection: IList sourceCollection } collectionView)
                    remove(a, sourceCollection);
            });

            DuplicateCommand ??= new Command<object>((a) =>
            {
                if (this.ItemsSource is IList collection)
                    add(a, collection);
                else if (this.ItemsSource is ICollectionView { SourceCollection: IList sourceCollection } collectionView)
                    add(a, sourceCollection);
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
            void add(object a, IList collection)
            {
                if (a is IClone clone)
                    collection.Add(clone.Clone());
                else
                    MessageBox.Show($"Failed to clone. {a.GetType()} does not derive from {nameof(IClone)}");
            }

            void swap(int[] indexes, IList collection)
            {
                int fromS = indexes[0];
                int to = indexes[1];
                var elementSource = collection[to];
                var dragged = collection[fromS];
                collection[to] = dragged;
                collection[fromS] = elementSource;
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

            if (Edit is { } _object)
            {
                routedEventArgs = new(isAccepted, _object, FinishEditEvent, this);
            }
            else
            {
                routedEventArgs = new(isAccepted, Edit, FinishEditEvent, this);
            }
            RaiseEvent(routedEventArgs);
            {
                this.GetBindingExpression(CustomSelector.EditProperty)?.UpdateTarget();
            }
        }

        public CustomSelector()
        {
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_CanExecute));
        }

        private void Copy_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            if (e.OriginalSource is ListBoxItem { DataContext: ICopy context })
            {
                Clipboard.SetText(context.Copy());
            }
            else if (e.OriginalSource is ListBox { SelectedItem: FrameworkElement { DataContext: ICopy _context } })
            {
                Clipboard.SetText(_context.Copy());
            }
            else if (e.OriginalSource is ListBox { SelectedItem: ICopy __context } )
            {
                Clipboard.SetText(__context.Copy());
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
