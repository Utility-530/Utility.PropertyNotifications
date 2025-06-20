using System;
using System.Windows;
using System.Windows.Controls;
using Utility.Interfaces.Exs;
using Utility.WPF.ResourceDictionarys;

namespace Utility.Nodes.Demo.Lists
{
    internal class HeaderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IGetNode { Node.DataTemplate: { } template })
            {
                if (Application.Current.Resources.FindResource(template) is DataTemplate dataTemplate)
                    return dataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
