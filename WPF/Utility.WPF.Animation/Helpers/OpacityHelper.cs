using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Utility.WPF.Animations.Helpers
{
    public static class OpacityHelper
    {
        const double FadeInTime = 0.25;
        const double FadeOutTime = 1;

        /// <summary>
        /// Fade the adorner in and make it visible.
        /// </summary>
        public static IObservable<EventArgs> FadeIn(this UIElement AssociatedObject, TimeSpan? timeSpan =  default)
        {
            ReplaySubject<EventArgs> replaySubject = new();
            DoubleAnimation doubleAnimation = new(1.0, new Duration(timeSpan??TimeSpan.FromSeconds(FadeInTime)));
            doubleAnimation.Completed += (s, e) => { replaySubject.OnNext(e); };
                doubleAnimation.Freeze();
            AssociatedObject.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
            return replaySubject;
        }

        /// <summary>
        /// Fade the adorner out and make it visible.
        /// </summary>
        public static IObservable<EventArgs> FadeOut(this UIElement AssociatedObject, TimeSpan? timeSpan = default)
        {
            ReplaySubject<EventArgs> replaySubject = new();
            DoubleAnimation fadeOutAnimation = new(0.0, new Duration(timeSpan ?? TimeSpan.FromSeconds(FadeOutTime)));
            fadeOutAnimation.Completed += (s, e) => { replaySubject.OnNext(e);  };
            fadeOutAnimation.Freeze();
            AssociatedObject.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
            return replaySubject;
        }
    }
}
