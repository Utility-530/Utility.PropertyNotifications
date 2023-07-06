using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Evan.Wpf;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Data;
using Utility.Interfaces.NonGeneric;
using System.ComponentModel;
using Utility.WPF.Controls.Objects.Infrastructure;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectComboBox : ComboBox
    {
        public static readonly DependencyProperty ObjectProperty = DependencyHelper.Register<object>(new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ValueProperty = DependencyHelper.Register();
        public static readonly DependencyProperty FiltersProperty = DependencyHelper.Register<IEnumerable>();

        static ObjectComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectComboBox), new FrameworkPropertyMetadata(typeof(ObjectComboBox)));
        }

        public ObjectComboBox()
        {
            //this.SetValue(ItemsControlEx.ArrangementProperty, Arrangement.Wrapped);
            var viewModel = new ObjectItemsControlViewModel();
            DataContext = viewModel;
            SetBinding(ObjectProperty, new Binding(nameof(ObjectItemsControlViewModel.Object)) { Mode = BindingMode.OneWayToSource });
            SetBinding(ValueProperty, new Binding(nameof(ObjectItemsControlViewModel.Value)) { Mode = BindingMode.OneWay });

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
}