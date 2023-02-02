using DynamicData;
using Evan.Wpf;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Reactive;
using UtilityWpf.Abstract;
using UtilityWpf.Controls.Master;
using UtilityWpf.Helper;

namespace UtilityWpf.Controls.Chart
{
    public class MasterChartDetail : MasterDetail
    {
        public static readonly DependencyProperty IdKeyProperty = DependencyHelper.Register();
        public static readonly DependencyProperty DataKeyProperty = DependencyHelper.Register();
        //public static readonly DependencyProperty DataProperty = DependencyHelper.Register();

        public MasterChartDetail()
        {
            var content = new OxyChart();

            this.WhenAnyValue(a => a.DataKey)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    content.DataKey = a;
                });

            this.WhenAnyValue(a => a.IdKey)
                .WhereNotNull()
                .Subscribe(a =>
                {
                    //selector.Key = a;
                    content.IdKey = a;
                });

            Content = content;
        }

        public string IdKey
        {
            get => (string)GetValue(IdKeyProperty);
            set => SetValue(IdKeyProperty, value);
        }

        public string DataKey
        {
            get => (string)GetValue(DataKeyProperty);
            set => SetValue(DataKeyProperty, value);
        }

        //public IEnumerable Data
        //{
        //    get => (IEnumerable)GetValue(DataProperty);
        //    set => SetValue(DataProperty, value);
        //}

        protected override IObservable<object> SelectFromMaster(Control control)
        {
            return control switch
            {
                ICheckedSelector selector => Collections(selector),
                _ => throw new Exception("556ds777f")
            };

            IObservable<IReadOnlyCollection<object>> Collections(ICheckedSelector selector)
            {
                return this.WhenAnyValue(a => a.IdKey)
                             .Select(prop =>
                                    selector.SelectCheckedAndUnCheckedItems()
                                        .WhereNotNull()
                                        .Select(a => a.AsObservableChangeSet(c => c.obj.GetType().GetProperty(prop ?? IdHelper.GetIdProperty(c.obj.GetType())).GetValue(c.obj)))
                                        .SelectMany(a => a))
                             .Switch()
                             .Filter(a => a.isChecked)
                             .Transform(a => a.obj)
                             .ToCollection();
            }
        }

        protected override void SetDetail(object content, object masterObject)
        {
            if (content is OxyChart oview && masterObject is IEnumerable enumerable)
            {
                oview.ItemsSource = enumerable;
                oview.DataKey = DataKey;
                oview.DataConverter = Converter;
                oview.IdKey = IdKey;
            }
            else throw new Exception("Content is null");
        }
    }
}