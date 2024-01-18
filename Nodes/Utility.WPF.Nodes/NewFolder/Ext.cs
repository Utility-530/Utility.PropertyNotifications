using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Utility.Nodes.Values;
using VisualJsonEditor.Test.Infrastructure;

namespace Utility.WPF.Nodes.NewFolder
{
    public static class Ext
    {
        public static ItemsPanel ToItemsPanel(this ViewModel viewModel)
        {
            return new ItemsPanel
            {
                Type = viewModel.ItemsPanelType,
                Orientation = Convert(viewModel.Orientation?? Enums.Orientation.Vertical),
                Rows = viewModel.Rows,
                Columns = viewModel.Columns,
                TemplateKey = viewModel.ItemsPanelTemplate,
            };
        }   

        public static DataPresentation ToDataPresentation(this ViewModel viewModel)
        {
            return new DataPresentation
            {
                Type = viewModel.DataPresentationType,
                Style = viewModel.DataPresentationStyle,
                TemplateKey = viewModel.DataTemplateKey,
            };
        }

        public static Orientation Convert(this Utility.Enums.Orientation orientation)
        {
            return orientation switch
            {
                Utility.Enums.Orientation.Horizontal=> Orientation.Horizontal,
                Utility.Enums.Orientation.Vertical=> Orientation.Vertical,
                _=> throw new Exception("!$^fgds 3432322")
            };
        }
    }
}
