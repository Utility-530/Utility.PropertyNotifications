using Microsoft.Xaml.Behaviors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Controls.DataGrids
{
    public class DataGridColumnSelectorBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty DataTemplateSelectorProperty =
            DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridColumnSelectorBehavior), new PropertyMetadata());


        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn +=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn);
        }

        public DataTemplateSelector DataTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(DataTemplateSelectorProperty); }
            set { SetValue(DataTemplateSelectorProperty, value); }
        }

        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            e.Column = SelectColumn(e);
            e.Column.Header = e.PropertyName;
        }

        protected virtual DataGridColumn SelectColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            var isReadonly = e.Column.IsReadOnly;
            //if (e.PropertyType.Equals(typeof(string)))
            //    return e.Column;

            if (e.Column is DataGridComboBoxColumn comboBoxColumn)
            {
                return new CustomComboBoxColumn() { Items = comboBoxColumn.ItemsSource, PropertyName = e.PropertyName };
            }

            var binding = (e.Column as DataGridBoundColumn)?.Binding;

            if (binding is null)
            {
                return e.Column;
            }

            DataGridColumn templateColumn = new CustomDataGridColumn()
            {
                Header = e.Column.Header,
                Binding = binding,
                PropertyType = e.PropertyType,
                DataContext = AssociatedObject.ItemsSource,
                DataTemplateSelector = DataTemplateSelector,
            };

            return templateColumn;
        }

    }

    public class CustomDataGridColumn : DataGridBoundColumn
    {
        public DataTemplateSelector DataTemplateSelector { get; set; }

        public CustomDataGridColumn()
        {
            IsReadOnly = false;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return new DataGridCell();
        }

        public object DataContext { get; set; }
        public Type PropertyType { get; internal set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (dataItem == CollectionView.NewItemPlaceholder)
            {
                return null;
            }

            //DataRowView dataRow = (dataItem as DataRowView);

            if (Binding is Binding _binding)
            {
                var binding = new Binding
                {
                    Source = dataItem,
                    Path = _binding.Path,
                    Mode = BindingMode.OneWay
                };
                if (PropertyType.Equals(typeof(string)) || PropertyType.IsValueType)
                {
                    binding.Mode = BindingMode.TwoWay;
                }

                var contentControl = new ContentControl() { ContentTemplateSelector = DataTemplateSelector };
                contentControl.SetBinding(ContentControl.ContentProperty, binding);
                return contentControl;
            }
            return new Ellipse { Fill = Brushes.Red, Width = 30, Height = 30 };
        }
    }


    public class CustomComboBoxColumn : DataGridColumn
    {
        public DataTemplateSelector DataTemplateSelector { get; set; }

        public CustomComboBoxColumn()
        {
            IsReadOnly = false;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return new DataGridCell();
        }

        public object DataContext { get; set; }
        public IEnumerable Items { get; set; }
        public string PropertyName { get; internal set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            if (dataItem == CollectionView.NewItemPlaceholder)
            {
                return null;
            }

            //DataRowView dataRow = (dataItem as DataRowView);

            System.Windows.Controls.ComboBox comboBox = new()
            {
                ItemsSource = Items
            };
            comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding { Source = dataItem, Path = new PropertyPath(PropertyName), Mode = BindingMode.TwoWay  });

            //var binding = new Binding
            //{
            //    Source = dataItem,
            //    Path = _binding.Path,
            //};
            return comboBox;

        }
    }
}
