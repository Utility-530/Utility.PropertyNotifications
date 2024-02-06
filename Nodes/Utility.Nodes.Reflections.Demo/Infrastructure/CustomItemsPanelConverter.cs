using System;
using System.Globalization;
using Utility.Enums;
using Utility.Models;
using Utility.Nodes.Reflections.Demo.Infrastructure;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using VisualJsonEditor.Test.Infrastructure;
using Utility.WPF.Nodes.NewFolder;
using Utility.ViewModels;

namespace Utility.Nodes.Demo
{
    public class CustomItemsPanelConverter : ItemsPanelConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is RootNode { })
            {
                return convert(new ItemsPanel
                {
                    Type = Arrangement.Stacked,
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                });
            }
            //if (value is IReadOnlyTree { Data: PropertyData { Descriptor: { PropertyType: { } propertyType, ComponentType: { } componentType, DisplayName: { } displayName } descriptor } baseObject })
            //{
            //    if (propertyType == typeof(LEDMessage))
            //    {
            //        return convert(new ItemsPanel
            //        {
            //            Type = Arrangement.Stacked,
            //            Orientation = System.Windows.Controls.Orientation.Horizontal,
            //        });
            //    }
            //}
            if (value is IReadOnlyTree { Key: Key { Guid: { } guid } key, Parent.Data: { } descriptor })
            {
                if (descriptor is CollectionItemDescriptor { Index: { } index } collectionItemDescriptor)
                {
                    return convert(new ItemsPanel
                    {

                        Type = Arrangement.Stacked,
                        Orientation = System.Windows.Controls.Orientation.Horizontal,
                    });
                }


                if (ViewModelStore.Instance.Get(guid) is ViewModel viewModel)
                {
                    var itemsPanel = viewModel.ToItemsPanel();
                    return convert(itemsPanel);
                }

                //var itemsPanel = baseObject.ToItemsPanel();
                //return convert(itemsPanel);
                return convert(new ItemsPanel
                {
                    Type = Arrangement.Stacked,
                    Orientation = System.Windows.Controls.Orientation.Vertical,
                });
            }

            if (value is CustomMethodsNode { })
            {
                return convert(new ItemsPanel
                {
                    Type = Arrangement.Stacked,
                    Orientation = System.Windows.Controls.Orientation.Horizontal,
                });
            }
            return base.Convert(value, targetType, parameter, culture);
        }

        public static CustomItemsPanelConverter Instance { get; } = new();
    }
}
