using LiveCharts;
using LiveCharts.Defaults;
using System;
using System.Linq;
using System.Windows.Controls;
using Utility.Helpers;
using UtilityWpf.Controls.Infrastructure;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for DateTimeUserControl.xaml
    /// </summary>
    public partial class DateTimeUserControl : UserControl
    {
        public DateTimeUserControl()
        {
            InitializeComponent();

            var prices = Finance.Prices.Sample(50).OrderBy(a => a.DateTime).ToArray();

            DateTimeControl1.Start = prices.First().DateTime;
            DateTimeControl1.End = prices.Last().DateTime;

            DateAxis.LabelFormatter = value => new DateTime((long)value).ToString("yyyy-MM:dd HH:mm:ss");

            DateTimeControl1.DateTimeRangeChanges().Subscribe(a =>
            {
                DateTimeControl1.Start = a.Range.Start;
                DateTimeControl1.End = a.Range.End;
                ChartValues<DateTimePoint> Values = new ChartValues<DateTimePoint>(), Values2 = new ChartValues<DateTimePoint>();
                foreach (Price dbe in prices.Where(ac => ac.DateTime > a.Range.Start && ac.DateTime < a.Range.End))
                {
                    Values.Add(new DateTimePoint(dbe.DateTime, dbe.Open));
                    Values2.Add(new DateTimePoint(dbe.DateTime, dbe.Close));
                }
                ScatterSeries.Values = Values;
                ScatterSeries2.Values = Values2;
            });
        }
    }
}