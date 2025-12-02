using System.Windows.Controls;
using System.Windows;
using Utility.WPF.Helpers;
using Utility.Models.Trees;
using Utility.Models;
using StyleSelector = System.Windows.Controls.StyleSelector;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes.Demo.Lists
{
    public class ContainerStyleSelector : StyleSelector
    {
        private bool isInitialised;
        private ResourceDictionary? res;

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is INodeViewModel { Style: "LabelStyle" })
                return LabelStyle;
            if (item is INodeViewModel { Style: "TableRowStyle" })
                return TableRowStyle;
            return item switch
            {
                _ => DefaultStyle ?? base.SelectStyle(item, container),
            };
        }

        public Style TableRowStyle { get; set; }
        public Style LabelStyle { get; set; }
        public Style DefaultStyle { get; set; }

        public static ContainerStyleSelector Instance { get; } = new();
    }
}