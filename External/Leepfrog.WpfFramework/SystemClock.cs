using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Threading;

namespace Leepfrog.WpfFramework
{
    public class SystemClock
    {
        public static event EventHandler NowChanged;

        private static DispatcherTimer _timer;

        public static DateTimeOffset Now
        {
            get
            {
                var ret = DateTimeOffset.Now;
                // IF TIMER ISN'T ALREADY RUNNING...
                if ( _timer == null )
                {
                    _timer = new DispatcherTimer(DispatcherPriority.Normal);
                    _timer.Tick += _timer_Tick;
                }
                if ( !_timer.IsEnabled )
                {
                    _timer.Interval = TimeSpan.FromMilliseconds(1000 - ret.Millisecond);
                    _timer.Start();
                }
                return ret;
            }
        }
        public static DateTimeOffset Tomorrow
        {
            get
            {
                return DateTimeOffset.Now.Date.AddDays(1);
            }
        }

        private static void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            NowChanged?.Invoke(null,EventArgs.Empty);
        }
    }
}
