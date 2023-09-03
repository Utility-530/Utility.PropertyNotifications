using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Utility.WPF.Controls.Objects.Infrastructure;
using System.Windows.Controls;
using Utility.Interfaces.NonGeneric;
using System.Globalization;
using Utility.Models;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectListBox : ListBox
    {
        public static readonly DependencyProperty ObjectProperty = DependencyHelper.Register<object>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register();
        public static readonly DependencyProperty FiltersProperty = DependencyHelper.Register<IEnumerable>();

        static ObjectListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectListBox), new FrameworkPropertyMetadata(typeof(ObjectListBox)));
        }

        public ObjectListBox()
        {
            var viewModel = new ObjectSelectionViewModel();

            this.WhenAnyValue(a => a.Object)
                .WhereNotNull()
                .Subscribe(a => viewModel.Object = a);

            viewModel.WhenAnyValue(a => a.Value)
                .WhereNotNull()
                .Subscribe(a => this.Value = a);

            var defaultView = CollectionViewSource.GetDefaultView(viewModel.Items);

            defaultView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));

            ItemsSource = defaultView;

            this.WhenAnyValue(a => a.Filters)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    defaultView.Filter = new Predicate<object>(o =>
                    {
                        bool b = true;
                        foreach (var x in a.OfType<IPredicate>())
                        {
                            if (x.Invoke(o) == false)
                                return false;
                        }
                        return b;
                    });
                    SelectedIndex = 0;
                });
        }


        public IEnumerable Filters
        {
            get => (IEnumerable)GetValue(FiltersProperty);
            set => SetValue(FiltersProperty, value);
        }
        public object Object
        {
            get => GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }

    public class ValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is ValueViewModel {Value:var _value } valueViewModel)
            {
                return _value;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
