using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Utility.WPF.Controls.Clocks
{
    public class AnalogClock : Clock
    {
        public static readonly DependencyProperty AngleSecondProperty = DependencyProperty.Register("AngleSecond", typeof(double), typeof(AnalogClock), new UIPropertyMetadata(default(double)));
        public static readonly DependencyProperty AngleMinuteProperty = DependencyProperty.Register("AngleMinute", typeof(double), typeof(AnalogClock), new UIPropertyMetadata(default(double)));
        public static readonly DependencyProperty AngleHourProperty = DependencyProperty.Register("AngleHour", typeof(double), typeof(AnalogClock), new UIPropertyMetadata(default(double)));

        public double AngleSecond
        {
            get { return (double)GetValue(AngleSecondProperty); }
            set { SetValue(AngleSecondProperty, value); }
        }

        public double AngleMinute
        {
            get { return (double)GetValue(AngleMinuteProperty); }
            set { SetValue(AngleMinuteProperty, value); }
        }

        public double AngleHour
        {
            get { return (double)GetValue(AngleHourProperty); }
            set { SetValue(AngleHourProperty, value); }
        }

        private DateTime lastTime;
        private Grid ClocksGrid;
        private Line LineSecond;
        private Line LineMinute;
        private Line LineHour;

        static AnalogClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnalogClock), new FrameworkPropertyMetadata(typeof(AnalogClock)));
        }

        public AnalogClock()
        {
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (ClocksGrid != null && LineHour != null && LineMinute != null && LineSecond != null)
            {
                ClocksGrid.Height = ClocksGrid.Width = Math.Min(arrangeBounds.Height, arrangeBounds.Width);

                LineSecond.Y1 = 3 * ClocksGrid.Height / 4;
                LineSecond.Y2 = ClocksGrid.Height / 4;

                LineHour.Y1 = 3 * ClocksGrid.Height / 5;
                LineHour.Y2 = ClocksGrid.Height / 4;

                LineMinute.Y1 = 2 * ClocksGrid.Height / 3;
                LineMinute.Y2 = ClocksGrid.Height / 4;
            }
            else
                throw new NullReferenceException("Missing controls when Arranging Override");

            return base.ArrangeOverride(arrangeBounds);
        }

        public override void OnApplyTemplate()
        {
            ClocksGrid = GetTemplateChild("ClocksGrid") as Grid;
            LineHour = GetTemplateChild("LineHour") as Line;
            LineMinute = GetTemplateChild("LineMinute") as Line;
            LineSecond = GetTemplateChild("LineSecond") as Line;

            OnTimeChanged(DateTime.Now);
            base.OnApplyTemplate();
        }

        protected override void OnTimeChanged(DateTime dateTime)
        {
            //if (dateTime.Second == lastTime.Second)
            //    return;
            var milliSecondInSeconds = dateTime.Millisecond / 1000d;
            AngleSecond = 6 * (dateTime.Second + milliSecondInSeconds);
            AngleMinute = 6 * dateTime.Minute + AngleSecond / 60;
            AngleHour = 30 * (dateTime.Hour % 12) + AngleMinute / 12;
            lastTime = dateTime;
        }
    }
}