using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Reactives;
//using Utility.Helpers.Ex;

namespace Utility.Models.Filters
{
    public class StringMatchFilter<T> : SubjectFilter<T>
    {
        private readonly Func<T, string, bool>[] array;
        private readonly IConnectableObservable<Unit> filters;
        private string filter;
        private readonly Subject<IChangeSet<T>> subjects = new();

        public StringMatchFilter() : base("String Match")
        {
            array = SelectPredicates().ToArray();
            filters = this.WhenAnyValue(a => a.Value).Select(a => Unit.Default).Replay(1);

            static IEnumerable<Func<T, string, bool>> SelectPredicates()
            {
                return typeof(T)
                     .GetProperties()
                     .Select(a => new Func<T, string, bool>((o, s) => Contains(a, o, s)));

                static bool Contains(System.Reflection.PropertyInfo propertyInfo, T o, string str)
                {
                    return str is not null &&
                        propertyInfo
                            .GetValue(o)?
                            .ToString()?
                            .Contains(str, StringComparison.InvariantCultureIgnoreCase) == true;
                }
            }
        }

        public override bool Evaluate(object value)
        {
            return array.Any(a => a.Invoke((T)value, Value.Value));
        }

        public override ReactiveProperty<string> Value { get; }



        public override void OnNext(IChangeSet<T> value)
        {
            subjects.OnNext(value);
        }

        public override IDisposable Subscribe(IObserver<Unit> observer)
        {
            filters.Connect();
            return filters
                .Subscribe(observer);
        }
    }
}
