using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Utility.WPF.ComboBoxes
{
    static class VisualTreeHelpers
    {
    
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
