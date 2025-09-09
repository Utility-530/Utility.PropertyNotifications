using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Nodify.Models;

namespace Utility.Nodify.Views.Infrastructure
{
    public class ConnectorTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is PendingConnectorViewModel)
            {
                return PendingTemplate;
            }
            return DefaultTemplate ?? base.SelectTemplate(item, container);
        }

        public DataTemplate? DefaultTemplate { get; set; }
        public DataTemplate? PendingTemplate { get; set; }

        public static ConnectorTemplateSelector Instance = new ConnectorTemplateSelector();
    }
}
