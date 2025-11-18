using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Wpf;
using Utility.Helpers.Ex;
using Utility.WPF.Abstract;

namespace Utility.WPF.Controls.Chart
{
    using System.Windows.Controls;
    using Evan.Wpf;
    using Utility.Reactives;
    using Utility.WPF.Controls.Chart.ViewModels;
    using Utility.WPF.Reactives;
    using static Utility.WPF.Controls.Chart.ViewModels.MultiTimeModel;

    public class OxyChart : Control, IItemsSource
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyHelper.Register<IEnumerable>();
        public static readonly DependencyProperty IdProperty = DependencyHelper.Register<string>();
        public static readonly DependencyProperty DataKeyProperty = DependencyHelper.Register<string>();
        public static readonly DependencyProperty DataConverterProperty = DependencyHelper.Register<IValueConverter>();
        private ReplaySubject<PlotView> replaySubject = new(1);

        static OxyChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OxyChart), new FrameworkPropertyMetadata(typeof(OxyChart)));
        }

        public override void OnApplyTemplate()
        {
            ;
            replaySubject.OnNext(this.FindName("PlotView1") as PlotView);
            base.OnApplyTemplate();
        }

        public OxyChart()
        {
            ISubject<MultiTimeModel> modelSubject = new Subject<MultiTimeModel>();

            var modelChanges =
                replaySubject
                .Subscribe(plotView =>
                {
                    plotView.Model ??= new PlotModel();
                    var model = new MultiTimeModel(plotView.Model);
                    modelSubject.OnNext(model);
                });

            modelSubject
                .CombineLatest(
                this.Observe(a => a.ItemsSource)
                .WhereIsNotNull()
                .Select(cc =>
                {
                    return cc;
                }),
                   this.Observe(a => a.DataKey),
                   this.Observe(a => a.DataConverter),
                this.Observe(a => a.Id).Where(id => id != null))
                .Subscribe(combination =>
                {
                    Combine(combination.First, combination.Second, combination.Third, combination.Fourth, combination.Fifth);
                });

            static void Combine(MultiTimeModel model, IEnumerable items, string dataKey, IValueConverter converter, string idKey)
            {
                if (items == default(IObservable<string>))
                    model.Filter(null);
                else

                {
                    HashSet<string> ids = GetIds(model, dataKey, converter, idKey, items);

                    model.Filter(ids);
                    var colors = model.SelectColors();
                }

                static HashSet<string> GetIds(MultiTimeModel model, string key, IValueConverter converter, string idKey, IEnumerable collection)
                {
                    // var itt = ItemsSource.Cast<object>().Select(o => o.GetType().GetProperty(IdProperty).GetValue(o).ToString());
                    HashSet<string> ids = new HashSet<string>();
                    foreach (var item in collection)
                    {
                        Color color = (Color)item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(Color))?.GetValue(item);
                        var aa = item.GetType().GetProperties().FirstOrDefault(a => a.PropertyType == typeof(TimeSpan));

                        TimeSpan? timeSpan = (TimeSpan?)aa?.GetValue(item);

                        var id = item.GetType().GetProperty(idKey).GetValue(item).ToString();
                        ids.Add(id);

                        if (color != default)
                        {
                            model.OnNext(new KeyValuePair<string, Color>(id, color));
                        }
                        if (timeSpan != default)
                        {
                            model.OnNext(new KeyValuePair<string, TimeSpan>(id, timeSpan.Value));
                        }

                        IEnumerable<DateTimePoint> data;
                        if (converter != null)
                        {
                            data = item.GetType().GetProperties().FirstOrDefault(a => a.Name == key)?.GetValue(item) as IEnumerable<DateTimePoint>;
                        }
                        else if (key != null)
                        {
                            data = item.GetType().GetProperties().FirstOrDefault(a => a.Name == key)?.GetValue(item) as IEnumerable<DateTimePoint>;
                        }
                        else
                        {
                            continue;
                            //return ids;
                        }

                        data
                            .ToObservable()
                            .Select(a => new KeyValuePair<string, (DateTime dt, double d)>(id, (a.DateTime, a.Value))).Buffer(TimeSpan.FromSeconds(0.5))
                            .Where(a => a.Count > 0)
                            .Subscribe(data =>
                            {
                                model.OnNext(data);
                            });
                    }

                    return ids;
                }
            }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string DataKey
        {
            get { return (string)GetValue(DataKeyProperty); }
            set { SetValue(DataKeyProperty, value); }
        }

        public IValueConverter DataConverter
        {
            get { return (IValueConverter)GetValue(DataConverterProperty); }
            set { SetValue(DataConverterProperty, value); }
        }

        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
    }
}