using System;
using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls
{
    public class DatesList : ListBox
    {
        static DatesList()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatesList), new FrameworkPropertyMetadata(typeof(DatesList)));
        }

        public DatesList()
        {
        }
    }

    public class DateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate YesterdayTemplate { get; set; }

        public DataTemplate TodayTemplate { get; set; }

        public DataTemplate TomorrowTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //FrameworkElement element = container as FrameworkElement;

            if (item != null && item is DateTime dateTime)
            {
                if (dateTime.IsYesterday())
                    return YesterdayTemplate;
                else if (dateTime.IsToday())
                    return TodayTemplate;
                else if (dateTime.IsTomorrow())
                    return TomorrowTemplate;
                else
                    return DefaultTemplate;
            }

            return null;
        }
    }

    internal static class DateTimeHelper
    {
        public static bool IsYesterday(this DateTime dt) => (dt >= DateTime.Today.AddDays(-1) && dt < DateTime.Today);

        public static bool IsTomorrow(this DateTime dt) => (dt <= DateTime.Today.AddDays(2) && dt > DateTime.Today.AddDays(1));

        public static bool IsToday(this DateTime dt) => DateTime.Today == dt.Date;
    }
}