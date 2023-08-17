using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Utility.Singletons;

namespace Utility.WPF.Attached
{
    public class TextBlockAttached
    {
        public static readonly DependencyProperty TimedTextProperty = DependencyProperty.RegisterAttached(
            "TimedText",
            typeof(string),
            typeof(TextBlockAttached),
            new FrameworkPropertyMetadata(string.Empty, TimedTextPropertyChanged));

        public static void SetTimedText(DependencyObject textBlock, string value)
        {
            textBlock.SetValue(TimedTextProperty, value);
        }

        public static string GetTimedText(DependencyObject textBlock)
        {
            return (string)textBlock.GetValue(TimedTextProperty);
        }

        private static Lazy<Chronic.Parser> parser = new(() => new());

        private static void TimedTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBlock textBlock)
            {
                return;
            }

            if (e.NewValue is DateTime dateTime)
            {
                Timer(dateTime, textBlock);
            }
            else if (e.NewValue is string str)
            {
                if (DateTime.TryParse(str, new CultureInfo("en-US"), DateTimeStyles.None, out var date))
                {
                    Timer(date, textBlock);
                    return;
                }
                if (parser.Value.Parse(str).ToTime() is DateTime cDate)
                {
                    Timer(cDate, textBlock);
                    return;
                }
            }
            throw new Exception("sdf,,.ddsd");
        }

        private static void Timer(DateTime dateTime, TextBlock textBlock)
        {
            TimerSingleton.Instance.Date.Connect();

            TimerSingleton.Instance.Date
                .Subscribe(dateNow =>
                {
                    textBlock.Text = (int)(dateNow - dateTime).TotalSeconds + " (s)";
                });
        }
    }
}