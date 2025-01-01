using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behaviors
{
    /// <summary>
    /// <see cref="https://stackoverflow.com/questions/22321966/interaction-triggers-in-style-in-resourcedictionary-wpf"/>
    /// </summary>
    public class FlipOnHover : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            Transform t = AssociatedObject.RenderTransform;

            AssociatedObject.RenderTransform = new TransformGroup();
            ((TransformGroup)AssociatedObject.RenderTransform).Children.Add(t);
            ((TransformGroup)AssociatedObject.RenderTransform).Children.Add(new ScaleTransform());
            base.OnAttached();
        }

        void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1]).ScaleY = 1;
        }

        void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1]).CenterX = AssociatedObject.ActualWidth / 2;
            ((ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1]).CenterY = AssociatedObject.ActualHeight / 2;
            ((ScaleTransform)((TransformGroup)AssociatedObject.RenderTransform).Children[1]).ScaleY = -1;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
        }
    }
}
