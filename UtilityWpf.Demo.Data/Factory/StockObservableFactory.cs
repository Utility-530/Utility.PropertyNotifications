using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Utility.Common;
using Utility.Helpers.Ex;
using Utility.Reactive;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Data.Factory
{
    public class StockObservableFactory
    {
        public static IObservable<IChangeSet<Stock, string>> GenerateChangeSet()
        {
            return Finance.Stocks
                .ToObservable()
                 .Buffer(5)
                 .Select(a => a.OrderBy(c => new Guid()).First())
                 .Pace(TimeSpan.FromSeconds(2))
                 .ToObservableChangeSet(c => c.Key);
        }

        public static IObservable<IChangeSet<Stock, string>> GenerateChangeSet1()
        {
            var innerSubject = new Subject<Stock>();
            var stocks = Finance.Stocks.ToObservable().Subscribe(innerSubject.OnNext);

            return innerSubject
                 .Buffer(5)
                 .Select(a => a.OrderBy(c => new Guid()).First())
                 .Pace(TimeSpan.FromSeconds(2))
                 .ToObservableChangeSet(c => c.Key);
        }

        /// <summary>
        /// Run for as long as unique stocks
        /// </summary>
        public static IObservable<IChangeSet<Groupable<Stock>, string>> GenerateLimitedGroupableChangeSet(IObservable<ClassProperty> subject) =>
            Finance.Stocks
            .Select(a => new Groupable<Stock>(a, new(nameof(Stock.Name), nameof(Stock)), subject))
            .ToObservable()
            .Buffer(5)
            .Select(a => a.OrderBy(c => new Guid()).First())
            .Pace(TimeSpan.FromSeconds(2))
            .ToObservableChangeSet(c => c.Value.Key)
            .DisposeMany();

        /// <summary>
        /// Will run forever
        /// </summary>
        public static IObservable<IChangeSet<Groupable<Stock>, string>> GenerateUnlimitedGroupableChangeSet(IObservable<ClassProperty> subject)
        {
            var innerSubject = new ReplaySubject<Stock>();
            var propType = new PropertyType<Stock>();
            foreach (var x in Finance.Stocks)
                innerSubject.OnNext(x);

            return innerSubject
                .Select(a => new Groupable<Stock>(a, propType, new(nameof(Stock.Name), nameof(Stock)), subject))
                .Buffer(5)
                .Select(a => a.OrderBy(c => new Guid()).First())
                .Pace(TimeSpan.FromSeconds(2))
                .ToObservableChangeSet(c => c.Value.Key)
                .DisposeMany();
        }
    }
}