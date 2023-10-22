using Evan.Wpf;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Helpers.Types;
using Utility.WPF.Abstract;
using Utility.WPF.Controls.Objects.Infrastructure;
using Utility.WPF.Controls.Objects;

namespace Utility.WPF.Demo.Objects
{
    /// <summary>
    /// Interaction logic for ObjectLevelUserControl.xaml
    /// </summary>
    public partial class ObjectLevelUserControl : UserControl
    {
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(ObjectLevelUserControl), new PropertyMetadata());
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(ObjectLevelUserControl), new PropertyMetadata(0));
        public ObjectLevelUserControl()
        {
            InitializeComponent();
            LevelComboBox.SelectionChanged += LevelComboBox_SelectionChanged;
            this.WhenAnyValue(a => a.Object)
                .Subscribe(a =>
                {
                    RefreshComboBox();
                });

            var viewModel = new ObjectSelectionViewModel();

            this.WhenAnyValue(a => a.Object)
                .WhereNotNull()
                .Subscribe(a => viewModel.Object = a);       
            
            this.WhenAnyValue(a => a.Level)
                .WhereNotNull()
                .Subscribe(a => viewModel.Level = a);

            viewModel.WhenAnyValue(a => a.Value)
                .WhereNotNull()
                .Subscribe(a => this.Value = a);

            var defaultView = CollectionViewSource.GetDefaultView(viewModel.Items);

            defaultView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));

            PropertiesControl.ItemsSource = defaultView;

            this.Object = new MyObject2();
            //this.WhenAnyValue(a => a.Filters)
            //    .WhereNotNull()
            //    .Subscribe(a =>
            //    {
            //        defaultView.Filter = new Predicate<object>(o =>
            //        {
            //            bool b = true;
            //            foreach (var x in a.OfType<IPredicate>())
            //            {
            //                if (x.Invoke(o) == false)
            //                    return false;
            //            }
            //            return b;
            //        });
            //        SelectedIndex = 0;
            //    });
        }

        private void LevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Level = Array.IndexOf((Array)LevelComboBox.ItemsSource, e.AddedItems[0]);

        }

        public object Object
        {
            get { return (object)GetValue(ObjectProperty); }
            set { SetValue(ObjectProperty, value); }
        }

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public void RefreshComboBox()
        {
            if (Object == null)
            {
                return;
            }
            Type[] types = Object.GetType().Ancestors().ToArray();
            LevelComboBox.ItemsSource = types;
        }


        //public static readonly DependencyProperty ObjectProperty = DependencyHelper.Register<object>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register();
        //public static readonly DependencyProperty FiltersProperty = DependencyHelper.Register<IEnumerable>();

        //static ObjectListBox()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectListBox), new FrameworkPropertyMetadata(typeof(ObjectListBox)));
        //}

        //public ObjectListBox()
        //{
         
        //}


        //public IEnumerable Filters
        //{
        //    get => (IEnumerable)GetValue(FiltersProperty);
        //    set => SetValue(FiltersProperty, value);
        //}
        //public object Object
        //{
        //    get => GetValue(ObjectProperty);
        //    set => SetValue(ObjectProperty, value);
        //}

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}

