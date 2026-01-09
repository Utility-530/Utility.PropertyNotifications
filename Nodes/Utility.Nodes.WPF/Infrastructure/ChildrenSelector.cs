using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Meta;
using Utility.Nodes.Ex;
using Utility.WPF.Controls.Trees;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.Models;

namespace Utility.Nodes.WPF
{
    public class ChildrenSelector : IChildrenSelector
    {
        public const string IsCollection = nameof(IsCollection);
        public IEnumerable Select(object data)
        {
            var tree = NodeExtensions.ToNodeViewModel([typeof(CustomModels.Controls).Assembly, new SystemAssembly(), new ReflectionAssembly()]);
            tree.Add(new Model { Name = IsCollection, Value = false });
            return tree;
        }
    }

    public class ChildrenSelectorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var childrenSelector = values.SingleOrDefault(a => a is IChildrenSelector);
            var node = values.SingleOrDefault(a => a is INodeViewModel);

            if (node != null)
            {
                if (childrenSelector is IChildrenSelector selector)
                {
                    return selector.Select(node);
                }
                if (node is IProliferation {  } proliferation)
                {
                    return proliferation.Proliferation();
                }
            }

            return values.SingleOrDefault(a => a is IEnumerable) ?? DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
