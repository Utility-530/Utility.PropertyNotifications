using DynamicData;
using ReactiveAsyncWorker.Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveAsyncWorker.ViewModel
{
    public class FactoryViewModel : ReactiveObject, IObserver<FactoryOrder>
    {
        private readonly ReplaySubject<FactoryOrder> newItemSubject = new ReplaySubject<FactoryOrder>();

        private readonly ReadOnlyObservableCollection<ReactivePair<FactoryStatus, IEnumerable<FactoryOrder>>> items;
        private readonly ObservableAsPropertyHelper<IEnumerable<FactoryOrder>> createdItems;
        private readonly ObservableAsPropertyHelper<IEnumerable<FactoryOrder>> scheduledItems;

        public FactoryViewModel(IObservable<FactoryOrder> factoryTag, IScheduler scheduler)
        {
            factoryTag.Subscribe(newItemSubject.OnNext);

            var changeSet = newItemSubject
                                .ObserveOn(scheduler)
                                .SubscribeOn(scheduler)
                                .ToObservableChangeSet(a => a.Key);

            var groups = changeSet.GroupWithImmutableState(a => a.State);

            var transforms = groups
                 .Transform(a =>
                 {
                     return ReactivePair<FactoryStatus, IEnumerable<FactoryOrder>>.Create(a.Key, a.Items);
                 });

            transforms
                .Bind(out items)
                .Subscribe();

            createdItems = transforms
                .Filter(a => a.Key == FactoryStatus.Created)
                .ToCollection()
                .Select(a => a.FirstOrDefault()?.Value ?? Array.Empty<FactoryOrder>())
                .ToProperty(this, a => a.CreatedCollection);

            scheduledItems = transforms
                .Filter(a => a.Key == FactoryStatus.Scheduled)
                .ToCollection()
                .Select(a => a.FirstOrDefault()?.Value ?? Array.Empty<FactoryOrder>())
                .ToProperty(this, a => a.ScheduledCollection);
        }

        public IEnumerable<FactoryOrder> CreatedCollection => createdItems.Value;

        public IEnumerable<FactoryOrder> ScheduledCollection => scheduledItems.Value;

        public ReadOnlyObservableCollection<ReactivePair<FactoryStatus, IEnumerable<FactoryOrder>>> CombinedCollection => items;

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(FactoryOrder value)
        {
            newItemSubject.OnNext(value);
        }
    }
}
