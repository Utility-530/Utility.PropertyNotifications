using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Enums;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using O = System.Windows.Controls.Orientation;

namespace Utility.Trees.Demo.MVVM.MVVM
{

        public class ItemsPanelConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is not IReadOnlyTree tree)
                {
                    return ItemsPanelFactory.Template(default, default, O.Vertical, Arrangement.Stacked);
                }

                //{
                //    if (tree.Data is ICollectionHeadersDescriptor { } _descriptor)
                //        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);

                //}
                {
                    if (tree.Data is PropertyDescriptor { } _descriptor)
                        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
                }
                {
                    if (tree.Data is ICollectionHeadersDescriptor { } _descriptor)
                        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
                }
                {
                    if (tree?.Data is ICollectionItemDescriptor { } _descriptor)
                        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
                }

                return ItemsPanelFactory.Template(default, default, O.Vertical, Arrangement.Stacked);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            public static ItemsPanelConverter Instance { get; } = new();

        }


}
