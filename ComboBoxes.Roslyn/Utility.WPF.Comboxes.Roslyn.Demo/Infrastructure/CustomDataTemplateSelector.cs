using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.ComboBoxes.Roslyn.Demo
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is EnumViewModel)
                return EnumTemplate;
            if (item is BooleanViewModel)
                return BooleanTemplate;

            return base.SelectTemplate(item, container);
        }

        public DataTemplate EnumTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
    }
}
