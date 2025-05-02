using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Leepfrog.WpfFramework.ExtensionMethods
{
    public static class VisualTreeExtensions
    {
        /// <summary>
        /// A simple iterator method to expose the visual tree to LINQ
        /// </summary>
        /// <param name="start">Parent from which to start searching</param>
        /// <param name="matchContainer">Must be null or return true to search chlidren of the supplied object</param>
        /// <param name="matchChild">Must be null or return true to return the supplied object</param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> EnumerateVisualTree(this DependencyObject start, Predicate<DependencyObject> matchContainer, Predicate<DependencyObject> matchChild)
        {
            return start.EnumerateVisualTree<DependencyObject>(matchContainer, matchChild);
        }

        /// <summary>
        /// A simple iterator method to expose the visual tree to LINQ
        /// </summary>
        /// <param name="start">Parent from which to start searching</param>
        /// <param name="matchContainer">Must be null or return true to search chlidren of the supplied object</param>
        /// <param name="matchChild">Must be null or return true to return the supplied object</param>
        /// <returns></returns>
        public static IEnumerable<T> EnumerateVisualTree<T>(this DependencyObject start, Predicate<DependencyObject> matchContainer, Predicate<T> matchChild) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                var child = VisualTreeHelper.GetChild(start, i);

                if (
                    (child is T typedChild)
                 && (
                     (matchChild == null)
                  || (matchChild(typedChild))
                    )
                    )
                {
                    yield return typedChild;
                }
                if (
                    (matchContainer == null)
                 || (matchContainer(child))
                   )
                {
                    foreach (var childOfChild in child.EnumerateVisualTree(matchContainer, matchChild))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
        public static IEnumerable<T> EnumerateVisualTree<T>(this DependencyObject start, Predicate<T> matchChild) where T : DependencyObject
        {
            return start.EnumerateVisualTree<T>(null, matchChild);
        }

    }

}
