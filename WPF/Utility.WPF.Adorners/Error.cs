using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Utility.WPF.Adorners
{
    public class Error
    {
        private static List<Guid> guids = new();

        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text", typeof(string), typeof(Error), new FrameworkPropertyMetadata(string.Empty, OnChanged));

        public static string GetText(UIElement adornedElement)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            return (string)adornedElement.GetValue(TextProperty);
        }

        public static void SetText(UIElement adornedElement, string placeholderText)
        {
            if (adornedElement == null)
                throw new ArgumentNullException("adornedElement");
            //offset = -adornedElement.Height;
            adornedElement.SetValue(TextProperty, placeholderText);
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Text.AddAdorner(sender, (string)e.NewValue, guids, (f, t) => new Text(f, t, Dock.Right, Place.Outside, Brushes.Red));
        }
    }
}