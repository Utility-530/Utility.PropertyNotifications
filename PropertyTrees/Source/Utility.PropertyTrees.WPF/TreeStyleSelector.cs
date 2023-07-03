using System.Windows;
using System.Windows.Controls;

namespace Utility.PropertyTrees.WPF
{
    public class TreeStyleSelector : StyleSelector
    {
        private static Style itemsOnlyStyle, headerOnlyStyle;
        const int columnWidth = 70;

        public static Style HeaderOnlyStyle
        {
            get
            {
                if (headerOnlyStyle != default)
                    return headerOnlyStyle;
                headerOnlyStyle = new Style { TargetType = typeof(TreeViewItem), BasedOn = null };
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };
                var factory = new FrameworkElementFactory(typeof(ContentPresenter));
                factory.SetValue(Control.NameProperty, "PART_Header");
                factory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
                factory.SetValue(ContentPresenter.WidthProperty, columnWidth * 1d);
                factory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                factory.SetValue(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
                factory.SetValue(Control.MarginProperty, new Thickness(2d));
                template.VisualTree = factory;
                var setter = new Setter { Property = Control.TemplateProperty, Value = template };
                headerOnlyStyle.Setters.Add(setter);
                return headerOnlyStyle;
            }
        }

        public static Style ItemsOnlyStyle
        {
            get
            {
                if (itemsOnlyStyle != default)
                    return itemsOnlyStyle;
                itemsOnlyStyle = new Style { TargetType = typeof(TreeViewItem) };
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };
                var factory = new FrameworkElementFactory(typeof(ItemsPresenter));
                factory.SetValue(Control.NameProperty, "ItemsHost");
                template.VisualTree = factory;
                var setter = new Setter { Property = Control.TemplateProperty, Value = template };
                itemsOnlyStyle.Setters.Add(setter);
                return itemsOnlyStyle;
            }
        }



        public override Style SelectStyle(object item, DependencyObject container)
        {

            if (item is TreeViewItem{  Header : PropertyBase { } propertyBase })
            {

                if(propertyBase.IsDescendantOfCollection())
                {
                    if(propertyBase.IsCollection)
                    {
                        return ItemsOnlyStyle;
                    }
                    if (propertyBase is ValueProperty valueProperty)
                    {
                        return HeaderOnlyStyle;
                    }
                    if (propertyBase.IsChildOfCollection())
                    {
                        return ItemsOnlyStyle;
                    }
                }


            }

            return base.SelectStyle(item, container);
        }
    }
}
