using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Utility.WPF.Adorners.Infrastructure
{
    public class AdornerBehavior : Behavior<UIElement>
    {
        private EllipseAdorner? adorner;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(AdornerBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseEnter += Element_MouseEnter;
            AssociatedObject.MouseLeave += Element_MouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.MouseEnter -= Element_MouseEnter;
                AssociatedObject.MouseLeave -= Element_MouseLeave;
            }
        }

        private void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            AdornerLayer? adLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);

            adorner = new EllipseAdorner(AssociatedObject, Command);
            adorner.MouseLeave += Ad_MouseLeave;

            adLayer?.Add(adorner);
        }

        private void Ad_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AdornerLayer.GetAdornerLayer(AssociatedObject) is { } adornerLayer)
            {
                foreach (var ellipseAdorner in adornerLayer.GetAdorners(AssociatedObject)?.OfType<EllipseAdorner>().ToArray() ?? Array.Empty<EllipseAdorner>())
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(3)));
                    doubleAnimation.Completed += fadeOutAnimation_Completed;
                    doubleAnimation.Freeze();

                    ellipseAdorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
                }
            }
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            if (adorner != null && adorner?.IsMouseOver == false)
            {
                if (AdornerLayer.GetAdornerLayer(AssociatedObject) is { } _ && AdornerLayer.GetAdornerLayer(AssociatedObject)?.GetAdorners(AssociatedObject) != null)
                {
                    foreach (var ellipseAdorner in AdornerLayer.GetAdornerLayer(AssociatedObject)?.GetAdorners(AssociatedObject)?.OfType<EllipseAdorner>().ToArray() ?? Array.Empty<EllipseAdorner>())
                    {
                        DoubleAnimation doubleAnimation = new DoubleAnimation(0.0, new Duration(TimeSpan.FromSeconds(1)));
                        doubleAnimation.Completed += fadeOutAnimation_Completed;
                        doubleAnimation.Freeze();

                        ellipseAdorner.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
                    }
                }
            }
        }

        private void fadeOutAnimation_Completed(object? sender, EventArgs e)
        {
            if (adorner != null)
            {
                if (AdornerLayer.GetAdornerLayer(AssociatedObject) is { } adornerLayer)
                {
                    adorner.MouseLeave -= Ad_MouseLeave;
                    adornerLayer.Remove(adorner);
                }
            }
        }
    }
}