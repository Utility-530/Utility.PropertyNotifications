using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Utility.WPF.Demo.Data.Model;
using static Utility.WPF.Controls.Chart.ViewModels.MultiTimeModel;

namespace Utility.WPF.Chart.Demo.ViewModels
{
    internal class ChartDetailsViewModel
    {
        //private ReadOnlyObservableCollection<KeyValuePair<string, DateTimePoint>> collection;
        private readonly Random random = new();

        private ReadOnlyObservableCollection<ChartDetailViewModel> collection;

        public ChartDetailsViewModel()
        {
            var resource = (Application.Current.FindResource("Characters") as IEnumerable).Cast<Character>().ToArray();

            _ = Observable
                .Generate(
                Enumerable.Repeat(1, 100000000).Select((a, i) => (i, resource[random.Next(0, resource.Length)])).GetEnumerator(),
                a => a.MoveNext(), a => a, a => a.Current.Item2, a => TimeSpan.FromSeconds(0.01 * a.Current.i))
                .Select((a, i) =>
                            new KeyValuePair<Character, DateTimePoint>(a, new DateTimePoint(DateTime.Now, 1d / (i + 1) + random.NextDouble())))
                .ToObservableChangeSet(100)
                .ObserveOnDispatcher()
                .GroupOn(a => a.Key)
                .Transform(aa =>
                {
                    aa.List.Connect().Transform(a => a.Value).Bind(out var dat).Subscribe();
                    return new ChartDetailViewModel(aa.GroupKey.First, aa.GroupKey.Color, dat);
                })
                .Bind(out collection)
                .Subscribe(a =>
                {
                });
        }

        public IEnumerable Collection => collection;

        public string DataKey => nameof(ChartDetailViewModel.Data);
    }
}