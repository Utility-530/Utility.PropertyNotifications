using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Utility.Entities;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal class CollectionViewService : IObserver<InList>, IObservable<ViewList>, IObserver<FilterPredicate>
    {
        ReplaySubject<InList> replaySubject = new();
        ReplaySubject<ViewList> viewReplaySubject = new();
        ReplaySubject<FilterPredicate> fSubject = new();

        public CollectionViewService()
        {
            replaySubject.Subscribe(a =>
            {
                var x = new ListCollectionView(a.Collection);

                //x.Filter = 
                viewReplaySubject.OnNext(new ViewList(x));
            });

            viewReplaySubject.CombineLatest(fSubject).Subscribe(x =>
            {
                (x.First.Collection as ListCollectionView).Filter = x.Second.Value;
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

        public void OnNext(InList value)
        {
            replaySubject.OnNext(value);
        }

        public void OnNext(FilterPredicate value)
        {
            fSubject.OnNext(value);

        }

        public IDisposable Subscribe(IObserver<ViewList> observer)
        {
            return viewReplaySubject.Subscribe(observer);
        }
    }
}
