using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Splat;
using Utility;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Nodes;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions.Async;
using Utility.WPF.Trees;

namespace Utility.WPF.Trees
{
    public class ChildrenConverter : IValueConverter
    {
        private INodeRoot? source;

        public ChildrenConverter()
        {
            source = Locator.Current.GetService<INodeRoot>();
        }

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
                tree.SetKey((GuidKey)tree.Type.GUID);
                tree.IsExpanded = true;
                source.Create(tree).Subscribe(a =>
                {
                    a.Descendants().Subscribe(d =>
                    {
                        (d.NewItem as INodeViewModel).IsExpanded = true;
                    });
                }); 
                //if (tree is IYieldItems yieldItems)
                //    foreach (var item in yieldItems.Items())
                //    {
                //        if (item is INodeViewModel _tree)
                //        {
                //            _tree.SetParent(tree);
                //            tree.Add(item);
                //            load(item);
                //        }
                //        tree.AreChildrenLoaded = true;
                //    }
                return tree.Children;
            }
            else
                throw new Exception("£34vfgdf 32");
        }
    }
}