using Bogus.DataSets;
using Endless;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utility.Trees.Abstractions;

namespace Utility.Trees.Demo.Infrastructure
{
    public class StateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var tree = values[0] as ITree;
            var tree1 = values[1] as ITree;
            if (tree == null || tree1 == null)
                return State.Default;
            return GetState(tree, tree1);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        State GetState(ITree one, ITree two)
        {
            var i0 = one.Index.ToString();
            var i1 = two.Index.ToString();
            var index0 = index(one);
            var index1 = index(two);

            if (index(one)==0 && one.Parent == two)
            {
                return State.Down;
            }
            if (one.HasItems && one[0] == two)
            {
                return State.Up;
            }
            if (one.Parent?.HasItems == true && two.Parent == one.Parent && index(one) + 1 == index(two))
            {
                return State.Back;
            }
            if (one.Parent?.HasItems == true && two.Parent == one.Parent && index(one) - 1 == index(two))
            {
                return State.Forward;
            }
            if (one == two)
            {
                return State.Current;
            }

            return State.Default;

        }

        int index(ITree tree)
        {
            return tree.Parent?.Items.IndexOf(tree) ?? -1;
        }
    }

    static class Helpoe
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T element)
        {
            int num = 0;
            foreach (T item in source)
            {
                if (item.Equals(element))
                {
                    return num;
                }

                num++;
            }

            return -1;
        }
    }
    //public ITree Up => Current.Parent;

    //public ITree Down => Current[0];

    //public ITree Forward => children[Index + 1];

    //public ITree Back => children[Index - 1];
}
