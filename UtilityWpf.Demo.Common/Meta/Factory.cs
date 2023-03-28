using Microsoft.Xaml.Behaviors.Core;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Common;
using Utility.Helpers;
using UtilityWpf.Demo.Common.ViewModels;

namespace UtilityWpf.Demo.Common.Meta
{
    public class FactoryLogger
    {
        public record FactoryLog(DateTime Date, string Key, string Type);

        // readonly Kaos.Collections.RankedSet<FactoryLog> logs = new Kaos.Collections.RankedSet<FactoryLog>(Comparer<FactoryLog>.Create(new Comparison<FactoryLog>((a, b) => (int)(a.Date - b.Date).Ticks)));
        private readonly ObservableCollection<FactoryLog> logs = new ObservableCollection<FactoryLog>();

        public void Add(DateTime Date, string Key, string Type)
        {
            logs.Add(new FactoryLog(Date, Key, Type));
        }

        public IReadOnlyCollection<FactoryLog> Logs => logs;
    }

    public class Factory
    {
        private FactoryLogger logger = Resolver.Instance.Resolve<FactoryLogger>();

        public T Create<T>() where T : class
        {
            var type = typeof(T);
            if (type == typeof(TickViewModel))
            {
                var t = Shelf.TickViewModel;
                logger.Add(DateTime.Now, t.Text, typeof(T).Name);
                return (t as T)!;
            }
            else if (type == typeof(ButtonViewModel))
            {
                var t = Shelf.ButtonViewModel;
                logger.Add(DateTime.Now, t.Header, typeof(T).Name);
                return (t as T)!;
            }
            else if (type == typeof(Fields))
            {
                var t = Shelf.Fields;
                logger.Add(DateTime.Now, t.Name, typeof(T).Name);
                return (t as T)!;
            }
            throw new ArgumentOutOfRangeException("s,,,,dfsdf");
        }

        public IEnumerable<T> Create<T>(int count) where T : class => EnumerableHelper.Create(count, Create<T>);

        private class Shelf
        {
            private static KeyStore Store = Resolver.Instance.Resolve<KeyStore>();

            public static TickViewModel TickViewModel => new(Guid.NewGuid(), Store.CreateNewKey()) { IsChecked = Statics.Random.NextBoolean() };
            public static ButtonViewModel ButtonViewModel => new(Store.CreateNewKey(), new ActionCommand(() => { })) { IsRefreshable = Statics.Random.NextBoolean() };

            public static Fields Fields => new Fields
            {
                Age = Statics.Random.Next(0, 100),
                Id = Guid.NewGuid(),
                Name = Statics.Random.NextWord(),
                Surname = Statics.Random.NextWord()
            };
        }
    }
}