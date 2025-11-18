using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Utility.PropertyNotifications;
using Utility.WPF.Controls.Date.Helper;

namespace Utility.WPF.Controls.Date.Model
{
    public abstract class DateModel : NotifyPropertyClass
    {
        private int year, month, day;
        protected readonly ComparableModel<DateTime> comparableModel = new();

        protected DateModel()
        {
            Current = DateTime.Now;
            this.WithChangesTo(a => a.Year)
                .CombineLatest(this.WithChangesTo(a => a.Month))
               .Subscribe(_ => RefreshDays());
        }

        public abstract void RefreshDays();

        public int Year
        {
            get => year;
            set => this.RaisePropertyChanged(ref year, value);
        }

        public int Month
        {
            get => month;
            set => this.RaisePropertyChanged(ref month, value);
        }

        public DateTime Current
        {
            get => RectifyDateTime();
            set
            {
                year = value.Year; month = value.Month; day = value.Day;
                RefreshDays();
            }
        }

        private DateTime RectifyDateTime()
        {
            while (true)
            {
                try
                {
                    return new DateTime(year, month, day);
                }
                // some months are longer than others
                catch (ArgumentOutOfRangeException)
                {
                    day--;
                }
            }
        }

        public ObservableCollection<DateTime> Days => comparableModel.Collection;
    }

    public class DateMonthModel : DateModel
    {
        public override void RefreshDays()
        {
            var days = DateHelper.VisibleDays(Month, Year).ToArray();
            comparableModel.Replace(days);
        }
    }

    public class DateRangeModel : DateModel
    {
        private DateTime dateTime;

        public override void RefreshDays()
        {
            if (dateTime == Current)
                return;

            dateTime = Current;

            var days = dateTime.PlusMinusDateRange().ToArray();
            comparableModel.Replace(days);
        }
    }
}