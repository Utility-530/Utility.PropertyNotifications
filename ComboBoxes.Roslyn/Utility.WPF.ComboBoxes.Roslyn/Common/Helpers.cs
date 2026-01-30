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

        public static T FindParent<T>(this DependencyObject obj) where T : DependencyObject
        {
            return obj.GetAncestors().OfType<T>().FirstOrDefault();
        }
        public static T FindChild<T>(this DependencyObject obj) where T : DependencyObject
        {
            return obj.GetDescendents().OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject element)
        {           
            do
            {
                yield return element;
                element = VisualTreeHelper.GetParent(element);
            } while (element != null);
        }
        public static IEnumerable<DependencyObject> GetDescendents(this DependencyObject source)
        {
            return source
              .GetVisualChildren()
              .SelectRecursive(element => element.GetVisualChildren());
        }
        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject source)
        {

            int count = VisualTreeHelper.GetChildrenCount(source);

            for (int i = 0; i < count; i++)
            {
                yield return VisualTreeHelper.GetChild(source, i);
            }
        }

        public static IEnumerable<TSource> SelectRecursive<TSource>(
         this IEnumerable<TSource> source,
         Func<TSource, IEnumerable<TSource>> recursiveSelector)
        {
            Contract.Requires(source != null);
            Contract.Requires(recursiveSelector != null);

            var stack = new Stack<IEnumerator<TSource>>();
            stack.Push(source.GetEnumerator());

            try
            {
                while (stack.Any())
                {
                    if (stack.Peek().MoveNext())
                    {
                        var current = stack.Peek().Current;

                        yield return current;

                        stack.Push(recursiveSelector(current).GetEnumerator());
                    }
                    else
                    {
                        stack.Pop().Dispose();
                    }
                }
            }
            finally
            {
                while (stack.Any())
                {
                    stack.Pop().Dispose();
                }
            }
        } //*** SelectRecursive
    }
}
