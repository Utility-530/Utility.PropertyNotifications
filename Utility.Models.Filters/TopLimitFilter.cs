using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Common.Models;
using Utility.Helpers.Ex;
using Utility.Reactive;

namespace Utility.Models.Filters
{
    public class TopLimitFilter<T> : SubjectFilter<T>
    {
        private readonly Subject<IChangeSet<T>> subjects = new();
        private readonly ReadOnlyObservableCollection<T> collection;

        public TopLimitFilter(int count) : base("Top")
        {
            Value = new ReactiveProperty<int>(count);

            subjects
                .Bind(out collection)
                .Subscribe();
        }

        public override bool Invoke(object value)
        {
            return 0 <= collection.IndexOf((T)value) && collection.IndexOf((T)value) < Value.Value;
        }

        public override ReactiveProperty<int> Value { get; }

        public override void OnNext(IChangeSet<T> value)
        {
            subjects.OnNext(value);
        }
        public override IDisposable Subscribe(IObserver<Unit> observer)
        {
            return Value.Select(a => Unit.Default).Subscribe(observer);
        }
    }
}
