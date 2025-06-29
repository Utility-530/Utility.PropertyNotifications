
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using UnitsNet;

namespace Utility.WPF.Controls
{
    public class Splashscreen : ContentControl
    {
        public static readonly DependencyProperty PercentProgressProperty =
    DependencyProperty.Register("PercentProgress", typeof(double), typeof(Splashscreen), new PropertyMetadata(0.0));

        public static readonly RoutedEvent FinishedEvent = EventManager.RegisterRoutedEvent("Finished", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Splashscreen));


        static Splashscreen()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Splashscreen), new FrameworkPropertyMetadata(typeof(Splashscreen)));
        }


        public event RoutedEventHandler Finished
        {
            add { AddHandler(FinishedEvent, value); }
            remove { RemoveHandler(FinishedEvent, value); }
        }

        DispatcherTimer delayTime = new DispatcherTimer();


        private const int timeSleep = 10;
        private const double realTime = 2; // real  time around 2 seconds
        private int circle = ((int)(realTime * 1000) / timeSleep);

        CancellationTokenSource source = new();

        public double PercentProgress
        {
            get { return (double)GetValue(PercentProgressProperty); }
            set { SetValue(PercentProgressProperty, value); }
        }



        public Splashscreen()
        {
            delayTime.Tick += new EventHandler(dt_Tick);
            delayTime.Interval = new TimeSpan(0, 0, 0, 0, 2400);
            process();
            delayTime.Start();
        }
        private async void process()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < circle + 20; i++)
                {
                    if (source.IsCancellationRequested)
                    {
                        return;
                    }
                    double a = (i / (double)circle) * 100;
                    Dispatcher.Invoke(() =>
                    {
                        PercentProgress = Math.Round(a, 1);
                    });
                    Task.Delay(timeSleep);
                }

            }, source.Token);
        }
        private void dt_Tick(object sender, EventArgs e)
        {
            delayTime.Stop();
            source.Cancel();
            RaiseEvent(new FinishedEventArgs(FinishedEvent));
        }
    }



    public class FinishedEventArgs : RoutedEventArgs
    {
        public FinishedEventArgs(RoutedEvent @event) : base(@event)
        {
        }
    }
}
