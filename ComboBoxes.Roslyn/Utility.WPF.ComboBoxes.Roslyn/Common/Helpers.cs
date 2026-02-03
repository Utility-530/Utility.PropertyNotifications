using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Utility.WPF.ComboBoxes.Roslyn
{
    static class Helpers
    {
        public static object? ElementAt(this IEnumerable source, int index)
        {
            if (source is Array col)
                return col.GetValue(index);

            int i = 0;
            var e = source.GetEnumerator();
            object? element = null;

            DynamicUsing(e, () =>
            {
                while (e.MoveNext())
                {
                    if (i == index)
                    {
                        element = e.Current;
                        break;
                    }
                    i++;
                }
            });

            return element;
        }

        public static void DynamicUsing(object resource, Action action)
        {
            try
            {
                action();
            }
            finally
            {
                (resource as IDisposable)?.Dispose();
            }
        }


        public static int Count(this IEnumerable source, Predicate<object>? predicate = null)
        {
            if (source is ICollection col)
                return col.Count;
            predicate ??= new Predicate<object>(a => true);
            int count = 0;
            var enumerator = source.GetEnumerator();
            DynamicUsing(enumerator, () =>
            {
                while (enumerator.MoveNext())
                    if (predicate(enumerator.Current))
                        count++;
            });

            return count;
        }     
    }
}
