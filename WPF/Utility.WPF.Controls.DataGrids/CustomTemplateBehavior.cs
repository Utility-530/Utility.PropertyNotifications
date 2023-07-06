using Castle.Components.DictionaryAdapter;
using Fasterflect;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.WPF.Helpers;
using Utility.WPF.Templates;

namespace Utility.WPF.Controls.DataGrids
{
    public class CustomTemplateBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty DataTemplateSelectorProperty =
            DependencyProperty.Register("DataTemplateSelector", typeof(DataTemplateSelector), typeof(CustomTemplateBehavior), new PropertyMetadata());


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
            var DG = (DataGrid)sender;
            e.Column = Make(e);
            e.Column.Header = e.PropertyName;

        }

        DataGridColumn Make(DataGridAutoGeneratingColumnEventArgs e)
        {
            Binding binding = (e.Column as DataGridBoundColumn)?.Binding as Binding;

            if(binding is null)
            {
                return e.Column;
            }
            DataGridColumn templateColumn = new CustomDataGridColumn()
            {
                Header = e.Column.Header,
                Binding = binding,
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

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var binding = Binding.Clone() as Binding;
            binding.Source = dataItem;
            var contentControl = new ContentControl() { ContentTemplateSelector = DataTemplateSelector };
            contentControl.SetBinding(ContentControl.ContentProperty, binding);
            return contentControl;
        }
    }
}
