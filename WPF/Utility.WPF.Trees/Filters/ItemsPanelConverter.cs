using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Enums;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using O = System.Windows.Controls.Orientation;

namespace Utility.Nodes.WPF
{

    public class ItemsPanelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not IReadOnlyTree tree)
            {
                return ItemsPanelFactory.Template(default, default, O.Vertical, Arrangement.Stacked);
            }
            if (value is IReadOnlyTree { Data: ICollectionItemReferenceDescriptor { Count: var count } descriptor })
            {
                return ItemsPanelFactory.Template(1, count, O.Horizontal, Arrangement.Uniform);
            }
            if (value is IReadOnlyTree { Data: ICollectionHeadersDescriptor { Count: var _count } })
            {
                return ItemsPanelFactory.Template(1, _count, O.Horizontal, Arrangement.Uniform);
            }
            {
                if (tree.Data is PropertyDescriptor { } _descriptor)
                    return ItemsPanelFactory.Template(1, default, O.Horizontal, Arrangement.Stacked);
            }
            //{
            //    if (tree.Data is ICollectionHeadersDescriptor { } _descriptor)
            //        return ItemsPanelFactory.Template(1, default, O.Horizontal, Arrangement.Uniform);
            //}
            //{
            //    if (tree?.Data is ICollectionItemDescriptor { } _descriptor)
            //        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
            //}

            return ItemsPanelFactory.Template(default, 1, O.Vertical, Arrangement.Stacked);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ItemsPanelConverter Instance { get; } = new();

    }


}
