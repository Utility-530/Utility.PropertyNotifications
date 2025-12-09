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
            if (item is INodeViewModel { Layout: {} layout })
            {
                switch(layout)
                {
                    case Enums.VisualLayout.Content:
                        return ContentStyle;
                    case Enums.VisualLayout.HeaderedPanel:
                        return HeaderedStyle;
                    case Enums.VisualLayout.TableRow:
                        return TableRowStyle;
                    case Enums.VisualLayout.Table:
                        return TableStyle;
                }                
            }
          
            //return item switch
            //{
            //    _ => HeaderedStyle ?? base.SelectStyle(item, container),
            //};
            //throw new Exception("F£$dff312");
            return HeaderedStyle;
        }

        public Style ContentStyle { get; set; }
        public Style TableRowStyle { get; set; }
        public Style TableStyle { get; set; }
        public Style HeaderedStyle { get; set; }

        public static ContainerStyleSelector Instance { get; } = new();
    }
}