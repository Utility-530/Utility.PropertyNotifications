using System.Windows;
using Tiny.Toolkits;

namespace Utility.WPF.Controls.Objects
{
    public class ObjectsDataTemplateSelector : Templates.CustomDataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item?.GetType().IsValueType == false && item is not string s)
            {
                var resource = container.GetResource<DataTemplate>("ObjectComboBoxTemplate");
                return resource;
            }

            return base.SelectTemplate(item, container);
        }

        public static new ObjectsDataTemplateSelector Instance { get; } = new();
    }
}