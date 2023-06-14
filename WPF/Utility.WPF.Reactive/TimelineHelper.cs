using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Utility.WPF.Reactive
{
    /// <summary>
    /// 
    /// </summary>
    public static class TimelineHelper
    {
        public static IObservable<EventArgs> SelectCompletions(this Timeline storyboard) =>

            Observable
            .FromEventPattern<EventHandler, EventArgs>
            (a => storyboard.Completed += a, a => storyboard.Completed -= a)
            .Select(a => a.EventArgs);

        public static Task<EventArgs> ToTask(this Timeline animation)
        {
            return SelectCompletions(animation).Take(1).ToTask();
        }

    }
}