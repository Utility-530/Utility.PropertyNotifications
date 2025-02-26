using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Lists
{
    internal class ItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var dataTemplate = Application.Current.TryFindResource(new DataTemplateKey(item.GetType())) as DataTemplate; 
            if(dataTemplate is DataTemplate dte)
            {
                return dte;
            }
            if (base.SelectTemplate(item, container) is DataTemplate dt)
            {
                return dt;
            }
            return DefaultTemplate;

        }

        public DataTemplate DefaultTemplate { get; set; }
    }
}
