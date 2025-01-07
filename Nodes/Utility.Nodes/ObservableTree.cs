using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Utility.Changes;
using Utility.Collections;
using Utility.Trees.Abstractions;

namespace Utility.Trees
{

    public abstract class ObservableTree : Tree, IObservable<Change<IReadOnlyTree>>
    {
        readonly ReplaySubject<Change<IReadOnlyTree>> subject = new();
        public IDisposable Subscribe(IObserver<Change<IReadOnlyTree>> observer)
        {
            return subject.Subscribe(observer);
        }

        protected override IList CreateChildren()
        {
            var collection = new Collection();
            collection.CollectionChanged += ItemsOnCollectionChanged;
            return collection;
        }

        protected override void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add && args.NewItems != null)
            {
                foreach (var item in args.NewItems.Cast<IReadOnlyTree>())
                {
                    item.Parent = this;
                    subject.OnNext(new Change<IReadOnlyTree>(item, Changes.Type.Add));
                    if (item is IObservable<Change<IReadOnlyTree>> c)
                        _ = c.Subscribe(subject);
                }
            }
            else if (args.Action != NotifyCollectionChangedAction.Move && args.OldItems != null)
            {
                foreach (var item in args.OldItems.Cast<Tree>())
                {
                    item.Parent = null;
                    subject.OnNext(new Change<IReadOnlyTree>(item, Changes.Type.Remove));
                }
            }
            base.ItemsOnCollectionChanged(sender, args);
        }
    }

}
