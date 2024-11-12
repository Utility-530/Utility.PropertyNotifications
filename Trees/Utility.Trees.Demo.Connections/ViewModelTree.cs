using System;
using System.Collections.Specialized;
using System.Reactive.Subjects;
using Utility.PropertyNotifications;

namespace Utility.Trees.Demo.Connections
{
    public class ViewModelTree : Tree, IObservable<Change>
    {
        ReplaySubject<Change> replaySubject = new();
        private string value;

        public ViewModelTree(object? data = null, params object[] items) : base(data, items)
        {
            if(data is ViewModel { } viewModel)
            {
                viewModel.WhenReceivedFrom(a => a.Value)
                    .Subscribe(a => replaySubject.OnNext(new Change(viewModel.Name, a)));
            }
        }

        protected override void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            foreach (var item in args.NewItems)
            {
                if (item is ViewModelTree {  } tree)
                {
                    tree.Subscribe(replaySubject);
                }
            }

            base.ItemsOnCollectionChanged(sender, args);
        }

        public IDisposable Subscribe(IObserver<Change> observer)
        {
            return replaySubject.Subscribe(observer);
        }
    }
}
