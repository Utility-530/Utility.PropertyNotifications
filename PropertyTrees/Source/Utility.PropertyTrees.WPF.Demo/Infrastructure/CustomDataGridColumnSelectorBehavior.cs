using Tiny.Toolkits;
using Utility.PropertyTrees.Services;
using Utility.WPF.Controls.DataGrids;

namespace Utility.PropertyTrees.WPF.Demo.Infrastructure
{
    public class CustomDataGridColumnSelectorBehavior : DataGridColumnSelectorBehavior
    {
        protected override DataGridColumn SelectColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            //if(e.PropertyName==nameof(ViewModel.ItemsPanelTemplateKey))
            //{
            //    //e.Column.GetResources<DataTemplate>();
            //    return new CustomComboBoxColumn() { Items = comboBoxColumn.ItemsSource, PropertyName = e.PropertyName };
            //}
            //if(e.PropertyName == nameof(ViewModel.DataTemplateKey))
            //{

            //    return new CustomComboBoxColumn() { Items = comboBoxColumn.ItemsSource, PropertyName = e.PropertyName };
            //}

            return base.SelectColumn(e);
        }
    }
}
