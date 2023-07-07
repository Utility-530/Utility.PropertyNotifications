using System.Windows;
using System.Windows.Controls;

namespace Utility.PropertyTrees.WPF
{
    public class TreeStyleSelector : StyleSelector
    {
        private static Style itemsOnlyStyle, fixedWidthHeaderOnlyStyle, headerOnlyStyle;
        const int columnWidth = 70;

        public static Style FixedWidthHeaderOnlyStyle
        {
            get
            {
                if (fixedWidthHeaderOnlyStyle != default)
                    return fixedWidthHeaderOnlyStyle;       
                fixedWidthHeaderOnlyStyle = new Style { TargetType = typeof(TreeViewItem) };      
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };
                template.VisualTree = MakeFactory();
                fixedWidthHeaderOnlyStyle.Setters.Add(new Setter(ContentPresenter.WidthProperty, columnWidth * 1d));            
                fixedWidthHeaderOnlyStyle.Setters.Add(new Setter { Property = Control.TemplateProperty, Value = template });
                return fixedWidthHeaderOnlyStyle;
            }
        }

        public static Style HeaderOnlyStyle
        {
            get
            {
                if (headerOnlyStyle != default)
                    return headerOnlyStyle;
                headerOnlyStyle = new Style { TargetType = typeof(TreeViewItem) };
                var template = new ControlTemplate { TargetType = typeof(TreeViewItem), };             
                template.VisualTree = MakeFactory();
                headerOnlyStyle.Setters.Add(new Setter { Property = Control.TemplateProperty, Value = template });
                return headerOnlyStyle;
            }
        }

        static FrameworkElementFactory MakeFactory()
        {
            var factory = new FrameworkElementFactory(typeof(ContentPresenter));
            factory.SetValue(Control.NameProperty, "PART_Header");
            factory.SetValue(ContentPresenter.ContentSourceProperty, "Header");
            factory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            factory.SetValue(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
            factory.SetValue(Control.MarginProperty, new Thickness(2d));
            return factory;
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
                        return FixedWidthHeaderOnlyStyle;
                    }
                    if (propertyBase.IsChildOfCollection())
                    {
                        return ItemsOnlyStyle;
                    }
                }

                if(propertyBase.PropertyType.Name=="ViewModelsCollection")
                {
                    return HeaderOnlyStyle;
                }
            }

            return base.SelectStyle(item, container);
        }
    }
}
