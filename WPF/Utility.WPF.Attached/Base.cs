using System;
using System.Collections;
using System.Windows;

namespace Utility.WPF.Attached
{
    public class Base
    {
        public static readonly DependencyProperty IntegerProperty = DependencyProperty.RegisterAttached(
        "Integer",
        typeof(int),
        typeof(Base),
        new FrameworkPropertyMetadata(int.MinValue, changed));

        private static void changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static void SetInteger(DependencyObject dep, int value)
        {
            dep.SetValue(IntegerProperty, value);
        }

        public static int GetInteger(DependencyObject dep)
        {
            return (int)dep.GetValue(IntegerProperty);
        }

        public static readonly DependencyProperty StringProperty = DependencyProperty.RegisterAttached(
        "String",
        typeof(string),
        typeof(Base),
        new FrameworkPropertyMetadata(null));

        public static void SetString(DependencyObject dep, string value)
        {
            dep.SetValue(StringProperty, value);
        }

        public static string GetString(DependencyObject dep)
        {
            return (string)dep.GetValue(StringProperty);
        }

        public static readonly DependencyProperty DoubleProperty = DependencyProperty.RegisterAttached(
        "Double",
        typeof(double),
        typeof(Base),
        new FrameworkPropertyMetadata(double.MinValue));

        public static void SetDouble(DependencyObject dep, double value)
        {
            dep.SetValue(DoubleProperty, value);
        }

        public static double GetDouble(DependencyObject dep) => (double)dep.GetValue(DoubleProperty);

        public static readonly DependencyProperty EnumerableProperty = DependencyProperty.RegisterAttached(
        "Enumerable",
        typeof(IEnumerable),
        typeof(Base),
        new FrameworkPropertyMetadata(Array.Empty<object>()));

        public static void SetEnumerable(DependencyObject dep, double value)
        {
            dep.SetValue(EnumerableProperty, value);
        }

        public static IEnumerable GetEnumerable(DependencyObject dep) => (IEnumerable)dep.GetValue(EnumerableProperty);
    }
}