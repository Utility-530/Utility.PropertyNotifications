using Splat;
using System;
using System.Collections.Generic;
using Utility.Helpers;
using IObserver = Utility.Models.IObserver;
using Utility.Repos;

namespace Utility.Trees.Demo.MVVM.Infrastructure
{
    public class PipeRepository : TreeRepository, IObserver<QueueItem>
    {


        public class D2Tree : DecisionTree<RepoQueueItem>
        {
            public D2Tree(IDecision decision, Func<RepoQueueItem, object>? transform = null) : base(decision, transform)
            {
            }
        }

        Dictionary<QueueItem, IObserver> dictionary = new();

        public Table? SelectedTable { get; set; }

        protected PipeRepository(string? dbDirectory = null) : base(dbDirectory)
        {
            Predicate = new DecisionTree(new Decision(item => (QueueItem)item != null) { })
            {
                new D2Tree(new Decision<RepoQueueItem>(item => item.ParentGuid == Guid.Parse("dbf5b684-894f-47ee-9b05-6b6e7a2ea931") && item.QueueItemType == QueueItemType.SelectKeys),

                    cdv =>
                    System.Reactive.Linq.Observable.Select( base.SelectKeys(),_keys =>
                            {
                                List<Key> keys = [];
                                foreach (var key in _keys)
                                {
                                    var table = new Table { Name = key.Name, Guid = key.Guid, Type = key.Instance.GetType() };
                                    keys.Add(key with { Instance = table });
                                }
                                return keys;
                            })
                ),
                new D2Tree(new Decision<RepoQueueItem>(item => item.ParentGuid == Guid.Parse("72022097-e5f6-4767-a18a-50763514ca01")&& item.QueueItemType == QueueItemType.SelectKeys),
                cdv =>
                    {
                        if (Locator.Current.GetService<Model>() is { SelectedTable:{ } selectedTable } )
                        {
                            return new List<Key>([new Key { Name = selectedTable.Name, Guid = selectedTable.Guid, Instance = Activator.CreateInstance(selectedTable.Type) }]);
                        }
                        return new object();
                    }

                ),
                new D2Tree(new Decision<RepoQueueItem>(item => true){  })
                {
                        new D2Tree(new Decision<RepoQueueItem>(item => item.QueueItemType == QueueItemType.Get){  }, qi=>base.Get(qi.Guid)                            ),

                        new D2Tree(new Decision<RepoQueueItem>(item => item.QueueItemType == QueueItemType.Find){  },
                        qi=>
                        base.Find(qi.Guid, qi.Name, qi.Type, qi.Index)

                        ),

                        new D2Tree(new Decision<RepoQueueItem>(item =>  item.QueueItemType == QueueItemType.SelectKeys){  },
                        qi=>
                        base.SelectKeys(qi.ParentGuid, qi.Name, qi.TableName)

                        ),
                },
            };
        }

        public override IObservable<DateValue?> Get(Guid guid)
        {
            var qi = new RepoQueueItem(guid, QueueItemType.Get);
            Pipe.Instance.Queue(qi);
            return dictionary.Get(qi, a => new Subject<DateValue?>()) as IObservable<DateValue?>;
        }

        public override IObservable<Guid> Find(Guid guid, string name, System.Type? type = null, int? index = null)
        {
            var qi = new RepoQueueItem(guid, QueueItemType.Find, name, type, index);
            Pipe.Instance.Queue(qi);
            return dictionary.Get(qi, a => new Subject<Guid>()) as Subject<Guid>; ;
        }

        public override IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null)
        {
            var qi = new RepoQueueItem(default, QueueItemType.SelectKeys, name, default, default, table_name, parentGuid);
            Pipe.Instance.Queue(qi);
            return dictionary.Get(qi, a => new Subject<IReadOnlyCollection<Key>>()) as Subject<IReadOnlyCollection<Key>>;
        }

        public DecisionTree Predicate { get; set; }

        public void OnNext(QueueItem queueItem)
        {
            Predicate.Reset();

            Predicate.Input = queueItem;

            Predicate.Evaluate();

            if (Predicate.Backput is IObservable<DateValue?> dt)
            {
                dt.Subscribe(a =>
                {
                    dictionary[Predicate.Input as QueueItem].OnNext(a);
                });
            }
            else if (Predicate.Backput is IObservable<Guid> dt2)
            {
                dt2.Subscribe(a =>
                {
                    dictionary[Predicate.Input as QueueItem].OnNext(a);
                });
            }
            else if (Predicate.Backput is IObservable<IReadOnlyCollection<Key>> dt3)
            {
                dt3.Subscribe(a =>
                {
                    dictionary[Predicate.Input as QueueItem].OnNext(a);
                });
            }

        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public static PipeRepository Instance2 { get; } = new(DataPath);
    }
}
