using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Utility.Trees.WPF
{
    public class TreeEx
    {
        public static readonly DependencyProperty RootProperty =
            DependencyProperty.RegisterAttached(
          "Root",
          typeof(object),
          typeof(TreeEx),
          new FrameworkPropertyMetadata(defaultValue: false,
              flags: FrameworkPropertyMetadataOptions.AffectsRender, Changed)
        );

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is TreeView treeView)
            {
                treeView.ItemsSource = new[] { e.NewValue };
            }
        }


        public static object GetRoot(UIElement target) => (object)target.GetValue(RootProperty);


        public static void SetRoot(UIElement target, object value) => target.SetValue(RootProperty, value);
    }
}
