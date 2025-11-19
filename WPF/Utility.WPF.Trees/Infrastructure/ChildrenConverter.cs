using System.Collections;
using System.Globalization;
using System.Windows.Data;
using Utility;
using Utility.Interfaces;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.WPF.Trees;

namespace Utility.WPF.Trees
{
    public class ChildrenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return load(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        IEnumerable load(object value)
        {
            if (value is INodeViewModel tree)
            {
                if (tree is IYieldItems yieldItems)
                    foreach (var item in yieldItems.Items())
                    {
                        if (item is INodeViewModel _tree)
                        {
                            _tree.SetParent(tree);
                            tree.Add(item);
                            load(item);
                        }
                        tree.AreChildrenLoaded = true;
                    }
                return tree.Children;
            }
            else
                throw new Exception("£34vfgdf 32");
        }
    }
}