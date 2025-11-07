using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using OxyPlot;
using OxyPlot.Wpf;
using RandomColorGenerator;
using Splat;
using Utility.Helpers;

namespace Utility.WPF.Controls.Chart.ViewModels
{
    public class MultiTimeModel :
        IObserver<IEnumerable<KeyValuePair<string, (DateTime date, double value)>>>,
        IObserver<KeyValuePair<string, (DateTime date, double value)>>,
        IObserver<KeyValuePair<string, Color>>,
        IObserver<KeyValuePair<string, TimeSpan>>

    {
        private Dictionary<string, CheckableList> SeriesDictionary;
        private Dispatcher dispatcher = Application.Current.Dispatcher;
        private PlotModel plotModel;
        private object lck = new object();
        private readonly Dictionary<string, Color> colorDictionary = new Dictionary<string, Color>();
        private readonly Dictionary<string, TimeSpan> timeSpanDictionary = new Dictionary<string, TimeSpan>();
        private ISubject<Unit> refreshes = new Subject<Unit>();

        public MultiTimeModel(PlotModel model)
        {
            plotModel = model;
            model.Axes.Add(new OxyPlot.Axes.DateTimeAxis { });
            SeriesDictionary = GetDataPoints();

            refreshes.Buffer(TimeSpan.FromSeconds(1)).Subscribe(list =>
            {
                lock (lck)
                {
                    var series = SelectSeries().AsParallel().ToArray();

                    dispatcher.InvokeAsync(() =>
                    {
                        plotModel.Series.Clear();
                        foreach (var serie in series)
                        {
                            plotModel.Series.Add(serie);
                        }
                        plotModel.InvalidatePlot(true);
                    });
                }
            });
        }

        private IEnumerable<OxyPlot.Series.LineSeries> SelectSeries()
        {
            foreach (var dataPoint in SeriesDictionary.Where(a => a.Value.Check))
            {
                var points = dataPoint.Value.DataPoints;

                //points = timeSpanDictionary.ContainsKey(dataPoint.Key.ToString()) ?
                //    Group(points, timeSpanDictionary[dataPoint.Key.ToString()]).ToList() :
                //    points;

                var build = Build(points.OrderBy(dt => dt.DateTime), dataPoint.Key.ToString(), SetColor(dataPoint.Key.ToString()));

                yield return build;
            }

            if (ShowAll)
            {
                var allPoints = SeriesDictionary.Where(a => a.Value.Check).SelectMany(a => a.Value.DataPoints)
                           .OrderBy(dt => dt.DateTime)
                           .GroupBy(a => a.DateTime)
                         .Select(xy0 => new DateTimePoint(xy0.Key, xy0.Sum(l => l.Value)));
                yield return Build(allPoints, "All", Splat.SplatColor.Black.ToNative());
            }
        }

        private static IEnumerable<DateTimePoint> Group(IEnumerable<DateTimePoint> dateTimePoints, TimeSpan timeSpan)
        {
            return dateTimePoints
                  .GroupBy(a => GroupKey(a.DateTime, timeSpan))
                 .Select(a => new DateTimePoint(a.Key, a.Average(v => v.Value)));

            static DateTime GroupKey(DateTime dt, TimeSpan timeSpan)
            {
                int factor = (int)((double)(dt - default(DateTime)).Ticks / timeSpan.Ticks) + 1;
                return default(DateTime) + timeSpan * factor;
            }
        }

        private Color SetColor(string v)
        {
            if (colorDictionary.ContainsKey(v) == false)
            {
                var color = RandomColor.GetColor(ColorScheme.Random, Luminosity.Dark);
                int max = 100, i = 0;
                while (colorDictionary.Values.Contains(color.ToNative()))
                {
                    color = RandomColor.GetColor(ColorScheme.Random, Luminosity.Dark);
                    if (++i > max)
                    {
                        color = System.Drawing.Color.Gray;
                        break;
                    }
                }
                colorDictionary[v] = color.ToNative();
            }

            return colorDictionary[v];
        }

        private Dictionary<string, CheckableList> GetDataPoints()
        {
            return new Dictionary<string, CheckableList>();
        }

        public bool ShowAll { get; set; } = false;

        public void OnNext(IEnumerable<KeyValuePair<string, (DateTime date, double value)>> enumerable)
        {
            lock (lck)
            {
                foreach (var item in enumerable)
                {
                    Add(item);
                }

                Refresh();
            }
        }

        public void OnNext(KeyValuePair<string, (DateTime date, double value)> item)
        {
            lock (lck)
            {
                Add(item);
                Refresh();
            }
        }

        public void OnNext(KeyValuePair<string, TimeSpan> kvp)
        {
            timeSpanDictionary[kvp.Key] = kvp.Value;
            Refresh();
        }

        public void OnNext(KeyValuePair<string, Color> kvp)
        {
            if (colorDictionary.ContainsKey(kvp.Key) == false || colorDictionary[kvp.Key] != kvp.Value)
            {
                colorDictionary[kvp.Key] = kvp.Value;
                Refresh();
            }
        }

        private void Add(KeyValuePair<string, (DateTime date, double value)> item)
        {
            if (!SeriesDictionary.ContainsKey(item.Key))
            {
                SeriesDictionary[item.Key] = new CheckableList();
                FilterRefesh();
            }
            var newdp = new DateTimePoint(item.Value.date, item.Value.value);

            SeriesDictionary[item.Key].DataPoints.Add(newdp);
        }

        private async void Refresh()
        {
            await Task.Run(() =>
            {
                refreshes.OnNext(new Unit());
            });
        }

        private Predicate<string> predicate = null;

        public void Filter(ISet<string> names)
        {
            predicate = names == null ? new Predicate<string>(s => true) : s => names.Contains(s);
            FilterRefesh();
        }

        private void FilterRefesh()
        {
            foreach (var kvp in SeriesDictionary)
            {
                kvp.Value.Check = predicate(kvp.Key);
            }
            lock (lck)
            {
                Refresh();
            }
        }

        public IEnumerable<KeyValuePair<string, Color>> SelectColors()
        {
            foreach (var series in plotModel.Series)
            {
                yield return new KeyValuePair<string, Color>(series.Title, (series as OxyPlot.Series.LineSeries).Color.ToColor());
            }
        }

        public void Reset()
        {
            dispatcher.Invoke(() =>
            {
                while (plotModel.Series.Any())
                    plotModel.Series.Remove(plotModel.Series.First());
                SeriesDictionary = GetDataPoints();
                plotModel.InvalidatePlot(true);
            });
        }

        public void Remove(ISet<string> names)
        {
            dispatcher.Invoke(() =>
            {
                while (plotModel.Series.Any(s => names.Contains(s.Title)))
                {
                    plotModel.Series.Remove(plotModel.Series.First());
                }
                SeriesDictionary = GetDataPoints();
                plotModel.InvalidatePlot(true);
            });
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public static OxyPlot.Series.LineSeries Build(IEnumerable<DateTimePoint> coll, string key)
        {
            var lser = new OxyPlot.Series.LineSeries
            {
                Title = key,
                StrokeThickness = 1,
                //Color = GetColor(key),
                MarkerSize = 3,
                ItemsSource = coll,
                MarkerType = MarkerType.Plus,
                DataFieldX = nameof(DateTimePoint.DateTime),
                DataFieldY = nameof(DateTimePoint.Value)
            };

            return lser;
        }

        public static OxyPlot.Series.LineSeries Build(IEnumerable<DateTimePoint> coll, string key, Color color)
        {
            var lser = new OxyPlot.Series.LineSeries
            {
                Title = key,
                StrokeThickness = 1,
                Color = color.ToOxyColor(),
                MarkerSize = 3,
                ItemsSource = coll,
                MarkerType = MarkerType.Plus,
                DataFieldX = nameof(DateTimePoint.DateTime),
                DataFieldY = nameof(DateTimePoint.Value)
            };

            return lser;
        }

        internal class CheckableList
        {
            public CheckableList(bool check, List<DateTimePoint> dataPoints)
            {
                Check = check;
                DataPoints = dataPoints;
            }

            public CheckableList(bool check = false) : this(check, new List<DateTimePoint>())
            {
            }

            public List<DateTimePoint> DataPoints { get; set; } = new List<DateTimePoint>();

            public bool Check { get; set; }
        }

        //
        // Summary:
        //     An already configured chart point with a date time and a double properties, this
        //     class notifies the chart to update every time a property changes
        public class DateTimePoint
        {
            //
            // Summary:
            //     Initializes a new instance of DateTimePoint class, giving date time and value
            //
            // Parameters:
            //   dateTime:
            //
            //   value:
            public DateTimePoint(DateTime dateTime, double value)
            {
                DateTime = dateTime;
                Value = value;
            }

            //
            // Summary:
            //     DateTime Property
            public DateTime DateTime { get; }

            //
            // Summary:
            //     The Value property
            public double Value { get; }
        }
    }
}