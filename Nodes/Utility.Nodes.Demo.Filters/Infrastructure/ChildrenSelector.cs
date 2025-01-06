using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TreeView.Infrastructure;
using Utility.Extensions;
using Utility.Meta;
using Utility.Trees;
using Utility.WPF.Controls.Trees;

namespace Utility.Nodes.Demo.Filters
{


    public class ChildrenSelector : IChildrenSelector
    {

        public IEnumerable Select(object data)
        {
            return NodeExtensions.ToTree([typeof(Tree).Assembly, new SystemAssembly()]);

        }
    }

    public class ChildrenSelectorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var childrenSelector = values.SingleOrDefault(a => a is IChildrenSelector);
            var items = values.SingleOrDefault(a => a is IEnumerable);
            var node = values.SingleOrDefault(a => a is Utility.Nodes.Filters.Node);

            if (node != null)
            {
                if (childrenSelector is IChildrenSelector selector)
                {
                    return selector.Select(node);
                }
                if (node is Utility.Nodes.Filters.Node { Data: IProliferation data })
                {
                    return data.Proliferation();
                }

            }
            if (items is IEnumerable)
            {
                return items;
            }

            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
