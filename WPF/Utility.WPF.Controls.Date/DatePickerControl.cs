using System.Windows;
using System.Windows.Controls;

namespace Utility.WPF.Controls.Date
{
    public class DateChangeEventArgs : RoutedEventArgs
    {
        public DateChangeEventArgs(RoutedEvent @event) : base(@event)
        { }
        public int Month { get; init; }
        public int Year { get; init; }
    }

    public delegate void DateChangeEventHandler(object sender, DateChangeEventArgs e);

    public class DatePickerControl : Control
    {

        public static readonly RoutedEvent DateChangeEvent =
            EventManager.RegisterRoutedEvent("DateChange", RoutingStrategy.Bubble, typeof(DateChangeEventHandler), typeof(DatePickerControl));

        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register("Month", typeof(int), typeof(DatePickerControl), new PropertyMetadata(1));

        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(DatePickerControl), new PropertyMetadata(0));

        static DatePickerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePickerControl), new FrameworkPropertyMetadata(typeof(DatePickerControl)));
        }

        public event DateChangeEventHandler DateChange
        {
            add { AddHandler(DateChangeEvent, value); }
            remove { RemoveHandler(DateChangeEvent, value); }
        }

        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }

        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            var yearDownButton = this.GetTemplateChild("YearDownButton") as Button;
            var yearUpButton = this.GetTemplateChild("YearUpButton") as Button;
            var monthDownButton = this.GetTemplateChild("MonthDownButton") as Button;
            var monthUpButton = this.GetTemplateChild("MonthUpButton") as Button;

            var yearTextBlock = this.GetTemplateChild("YearTextBlock") as TextBlock;
            var monthTextBlock = this.GetTemplateChild("MonthTextBlock") as TextBlock;
            var calendarDays = this.GetTemplateChild("CalendarDays") as ListBox;

            yearDownButton.Click += YearDownButton_Click;
            yearUpButton.Click += YearUpButton_Click;
            monthDownButton.Click += MonthDownButton_Click;
            monthUpButton.Click += MonthUpButton_Click;

            // model.PropertyChanged += Model_PropertyChanged;
            // Current = model.Current;
            //yearTextBlock.Text = model.Year.ToString();
            //monthTextBlock.Text = model.Current.ToString("MMMM");

            base.OnApplyTemplate();

            void YearUpButton_Click(object sender, RoutedEventArgs e)
            {
                Year--; RaiseEvent(new DateChangeEventArgs(DateChangeEvent) { Month = Month, Year = Year });
            }

            void YearDownButton_Click(object sender, RoutedEventArgs e)
            {
                Year++; RaiseEvent(new DateChangeEventArgs(DateChangeEvent) { Month = Month, Year = Year });
            }

            void MonthUpButton_Click(object sender, RoutedEventArgs e)
            {
                if (Month == 1)
                {
                    Year--;
                    Month = 12;
                }
                else
                {
                    Month--;
                }
                RaiseEvent(new DateChangeEventArgs(DateChangeEvent) { Month = Month, Year = Year });
            }

            void MonthDownButton_Click(object sender, RoutedEventArgs e)
            {
                if (Month == 12)
                {
                    Year++;
                    Month = 1;
                }
                else
                {
                    Month++;
                }
                RaiseEvent(new DateChangeEventArgs(DateChangeEvent) { Month = Month, Year = Year });
            }
        }
    }

}
