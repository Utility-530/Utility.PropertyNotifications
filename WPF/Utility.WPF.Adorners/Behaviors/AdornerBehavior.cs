using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;

namespace Utility.WPF.Adorners
{
    public class AdornerBehavior : Behavior<UIElement>
    {
        private readonly HashSet<Adorner> activeAdorners = new();
        private readonly Dictionary<Adorner, AnimationInfo> runningAnimations = new();

        // Helper class to track animation and its event handler
        private class AnimationInfo
        {
            public DoubleAnimation Animation { get; set; }
            public EventHandler CompletedHandler { get; set; }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(AdornerBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

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

                // Clean up any remaining adorners
                CleanupAllAdorners();
            }
        }

        protected virtual Adorner CreateAdorner() => new EllipseAdorner(AssociatedObject, Command);

        private bool isValid = true;
        private DispatcherTimer timer = null;

        private void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            if (adornerLayer == null) return;

            if (activeAdorners.Count > 0 || isValid == false)
            {
                if (activeAdorners.Any())
                    StopFadeOutAnimation(activeAdorners.First());
                return;
            }

            var adorner = CreateAdorner();
            Trace.WriteLine("bbbb");
            adorner.MouseEnter += Adorner_MouseEnter;
            adorner.MouseLeave += Adorner_MouseLeave;
            adorner.MouseLeftButtonDown += Adorner_MouseLeftButtonDown;

            activeAdorners.Add(adorner);
            adornerLayer.Add(adorner);
        }

        private void Element_MouseLeave(object sender, MouseEventArgs e)
        {
            // Only start fade out if mouse is not over any of the adorners
            if (!activeAdorners.Any(a => a.IsMouseOver))
            {
                StartFadeOutAnimation(TimeSpan.FromSeconds(1));
            }
        }

        private void Adorner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Adorner adorner)
            {
                // Execute the command if it exists and can execute
                if (Command?.CanExecute(null) == true)
                {
                    Command.Execute(null);
                }

                // Remove the adorner immediately on click
                StopFadeOutAnimation(adorner);
                _ = RemoveAdorner(adorner);

                // Mark the event as handled to prevent it from bubbling up
                e.Handled = true;
            }
        }

        private void Adorner_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Adorner adorner)
            {
                // Stop any running fade-out animation and restore opacity
                StopFadeOutAnimation(adorner);
                RestoreOpacity(adorner);
            }
        }

        private void Adorner_MouseLeave(object sender, MouseEventArgs e)
        {
            // Check if mouse is still over the associated object or any other adorner
            if (!AssociatedObject.IsMouseOver && !activeAdorners.Any(a => a.IsMouseOver))
            {
                StartFadeOutAnimation(TimeSpan.FromSeconds(3));
            }
        }

        private void StartFadeOutAnimation(TimeSpan duration)
        {
            var adornersToAnimate = activeAdorners.ToArray();

            foreach (var adorner in adornersToAnimate)
            {
                // Stop any existing animation for this adorner first
                StopFadeOutAnimation(adorner);

                var fadeOutAnimation = new DoubleAnimation(0.0, new Duration(duration));

                // Create a proper event handler that we can unsubscribe from
                EventHandler completedHandler = (s, e) => FadeOutAnimation_Completed(adorner);
                fadeOutAnimation.Completed += completedHandler;
                //fadeOutAnimation.Freeze();

                // Track both the animation and its event handler
                runningAnimations[adorner] = new AnimationInfo
                {
                    Animation = fadeOutAnimation,
                    CompletedHandler = completedHandler
                };

                adorner.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            }
        }

        private void StopFadeOutAnimation(Adorner adorner)
        {
            if (runningAnimations.TryGetValue(adorner, out var animationInfo))
            {
                // Properly unsubscribe from the completed event using the stored handler
                animationInfo.Animation.Completed -= animationInfo.CompletedHandler;

                // Stop the current animation
                adorner.BeginAnimation(UIElement.OpacityProperty, null);
                runningAnimations.Remove(adorner);
            }
        }

        private void RestoreOpacity(Adorner adorner)
        {
            var fadeInAnimation = new DoubleAnimation(adorner.Opacity, 1.0, new Duration(TimeSpan.FromSeconds(0.2)));
            // Don't freeze this animation either for consistency
            adorner.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        private void FadeOutAnimation_Completed(Adorner adorner)
        {
            // Only remove if the adorner still exists and has a running animation
            // This ensures we only respond to legitimate animation completions
            if (adorner != null &&
                activeAdorners.Contains(adorner) &&
                runningAnimations.ContainsKey(adorner))
            {
                runningAnimations.Remove(adorner);
                _ = RemoveAdorner(adorner);
            }
        }

        private async Task RemoveAdorner(Adorner adorner)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(AssociatedObject);
            if (adornerLayer != null)
            {
                // Clean up any running animation first
                StopFadeOutAnimation(adorner);

                adorner.MouseEnter -= Adorner_MouseEnter;
                adorner.MouseLeave -= Adorner_MouseLeave;
                adorner.MouseLeftButtonDown -= Adorner_MouseLeftButtonDown;
                adornerLayer.Remove(adorner);
                Trace.WriteLine("aaaa");
                activeAdorners.Remove(adorner);

                isValid = false;

                // Clean up existing timer if any
                timer?.Stop();

                timer = new DispatcherTimer(TimeSpan.FromSeconds(0.2), DispatcherPriority.Loaded,
                    (s, e) =>
                    {
                        isValid = true;
                        timer.Stop();
                        timer = null;
                    }, Dispatcher);
                timer.Start();
            }
        }

        private void CleanupAllAdorners()
        {
            var adornersToRemove = activeAdorners.ToArray();
            foreach (var adorner in adornersToRemove)
            {
                _ = RemoveAdorner(adorner);
            }
        }
    }
}