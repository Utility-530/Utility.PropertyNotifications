using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Utility.WPF.Demo.Panels
{
    internal static class Helper
    {
        public static bool AllDistinct<T>(this ICollection<T> collection)
        {
            return collection.Count == collection.Distinct().Count();
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                    return (T)child;
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        public static (int one, int two) GetLowestRatio(int number1, int number2)
        {
            var min = Math.Min(number1, number2);
            var number1IsMin = min == number1;
            var max = number1IsMin ? number2 : number1;
            int tempMin = min;
            while (tempMin > 1 && tempMin <= min)
            {
                var overMax = 1d * max / tempMin % 1;
                var overMin = 1d * min / tempMin % 1;
                if (overMax < double.Epsilon && overMin < double.Epsilon)
                {
                    min /= tempMin;
                    max /= tempMin;
                }
                else
                    tempMin--;
            }
            return number1IsMin ? (min, max) : (max, min);
        }
    }
}