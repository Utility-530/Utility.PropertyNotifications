using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.Models;
using Utility.Reactives;

namespace Utility.Nodify.Views.Infrastructure
{
    public class NodeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var type = item.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Observable<>))
            {
                if (ObservableTemplate is not null)
                    return ObservableTemplate;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueModel<>))
            {
                if (ValueModelTemplate is not null)
                    return ValueModelTemplate;
            }
            return base.SelectTemplate(item, container);
        }

        public DataTemplate ObservableTemplate { get; set; }
        public DataTemplate ValueModelTemplate { get; set; }
    }
}
