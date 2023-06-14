using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Behavior
{
    public class MouseWheelScrollBehavior : Behavior<UIElement>
    {


        public double Rate
        {
            get => (double)GetValue(RateProperty);
            set => SetValue(RateProperty, value);
        }

        public static readonly DependencyProperty RateProperty = DependencyProperty.Register("Rate", typeof(double), typeof(MouseWheelScrollBehavior), new PropertyMetadata(1d));


        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewMouseWheel -= PreviewMouseWheel;
            base.OnDetaching();
        }

        private void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta * Rate);
            e.Handled = true;
        }
    }
}
