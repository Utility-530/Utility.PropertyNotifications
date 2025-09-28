using System;
using System.Globalization;
using System.Windows.Data;
using Utility.Enums;
using Utility.Interfaces;
using Utility.Interfaces.Generic;
using Utility.PropertyDescriptors;
using Utility.Structs;
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
                return ItemsPanelFactory.Template(default, default, O.Vertical, Arrangement.Stack);
            }
            //if (value is IReadOnlyTree { Data: IReferenceDescriptor { Type: { } type, Count: int count }, Parent.Data: ICollectionDescriptor { } descriptor } && typeof(IEnumerable).IsAssignableFrom(type))
            //{
            //    return ItemsPanelFactory.Template(1, 1, O.Horizontal, Arrangement.Uniform);
            //}
            if (value is IReferenceDescriptor { Count: int _count } && value is IGetParent<IReadOnlyTree> { Parent: ICollectionDescriptor { } })
            {
                return ItemsPanelFactory.Template([new Dimension()] , Enumerable.Repeat(new Dimension(), Math.Max(_count, 1)).ToArray(), O.Horizontal, Arrangement.Uniform);
            }
            if (value is ICollectionHeadersDescriptor { Count: var __count } )
            {
                return ItemsPanelFactory.Template([new Dimension()], Enumerable.Repeat(new Dimension(), Math.Max(__count, 1)).ToArray(), O.Horizontal, Arrangement.Uniform);
            }
            {
                if (tree is IPropertyDescriptor { } _descriptor)
                    return ItemsPanelFactory.Template([new Dimension()], default, O.Horizontal, Arrangement.Stack);
            }
            //{
            //    if (tree.Data is ICollectionHeadersDescriptor { } _descriptor)
            //        return ItemsPanelFactory.Template(1, default, O.Horizontal, Arrangement.Uniform);
            //}
            //{
            //    if (tree?.Data is ICollectionItemDescriptor { } _descriptor)
            //        return ItemsPanelFactory.Template(default, default, O.Horizontal, Arrangement.Stacked);
            //}

            return ItemsPanelFactory.Template(default, [new Dimension()], O.Vertical, Arrangement.Stack);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static ItemsPanelConverter Instance { get; } = new();

    }


}
