using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Utility.WPF.Controls.DataGrids
{
    public class CustomTemplateBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty DataTemplateSelectorProperty =
            DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTemplateBehavior), new PropertyMetadata());


        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn +=
                new EventHandler<DataGridAutoGeneratingColumnEventArgs>(OnAutoGeneratingColumn)
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
            var DG = (DataGrid)sender;
            e.Column = Make(e);
            e.Column.Header = e.PropertyName;

        }

        DataGridColumn Make(DataGridAutoGeneratingColumnEventArgs e)
        {
            var binding = (e.Column as DataGridBoundColumn)?.Binding;

            if (binding is null)
            {
                return e.Column;
            }

            DataGridColumn templateColumn = new CustomDataGridColumn()
            {
                Header = e.Column.Header,
                Binding = binding,
                DataContext = AssociatedObject.ItemsSource,
                DataTemplateSelector = DataTemplateSelector
            };

            return templateColumn;
        }

    }

    public class CustomDataGridColumn : DataGridBoundColumn
    {
        public DataTemplateSelector DataTemplateSelector { get; set; }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return new DataGridCell();
        }

        public object DataContext { get; set; }

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
                };
                var contentControl = new ContentControl() { ContentTemplateSelector = DataTemplateSelector };
                contentControl.SetBinding(ContentControl.ContentProperty, binding);
                return contentControl;
            }
            return new Ellipse { Fill = Brushes.Red, Width = 30, Height = 30 };
        }
    }
}
