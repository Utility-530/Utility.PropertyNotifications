using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.PropertyTrees.WPF.Demo
{
    internal class ContentTemplateSelector:DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is not ValueProperty{ Descriptor.PropertyType: var propertyType } propertyNode) {
                throw new Exception("vdf fee333");
            }
            if(propertyType == typeof(string))
            {
                return StringTemplate;
            }
            if (propertyType == typeof(bool))
            {
                return BooleanTemplate;
            }
            if (propertyType == typeof(double))
            {
                return DoubleTemplate;
            }
            if (propertyType == typeof(int))
            {
                return IntegerTemplate;
            }
            //if (item is null)
            //{
            //    return TextBoxTemplate;
            //}
            return base.SelectTemplate(item, container);
        }
        public static ContentTemplateSelector Instance { get; } = new ContentTemplateSelector();

        public DataTemplate StringTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }
        public DataTemplate DoubleTemplate { get; set; }
        public DataTemplate IntegerTemplate { get; set; }
    }
}
