using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Utility.Helpers;
using Utility.Repos;

namespace Utility.Trees.Demo.MVVM.Infrastructure
{
    public class PipeRepository : TreeRepository, IObserver<QueueItem>
    {
        Dictionary<QueueItem, object> dictionary = new();

        public Table? SelectedTable { get; set; }

        protected PipeRepository(string? dbDirectory = null) : base(dbDirectory)
        {
            Predicate = new StringDecisionTree(new Decision(item => (QueueItem)item != null) { })
                {
                    new StringDecisionTree(new Decision<QueueItem>(item => item.ParentGuid == Guid.Parse("dbf5b684-894f-47ee-9b05-6b6e7a2ea931")), md=>"B"),
                    //new StringDecisionTree(new Decision<QueueItem>(item => item.Guid == Guid.Parse("307fcf2d-696e-45ec-89fe-6db94e02e9e6")), md=>"C"),
                    new StringDecisionTree(new Decision<QueueItem>(item => item.ParentGuid == Guid.Parse("1f51406b-9e37-429d-8ed2-c02cff90cfdb")), md=>"D"),
                    new StringDecisionTree(new Decision<QueueItem>(item => item.ParentGuid == Guid.Parse("72022097-e5f6-4767-a18a-50763514ca01")), md=>"D"),
                    //new StringDecisionTree(new Decision<QueueItem>(item => false), md=>"B"),
                    new StringDecisionTree(new Decision<QueueItem>(item => true){  }, md=>"A"),

                };
        }

        public override IObservable<DateValue?> Get(Guid guid)
        {
            if(guid== Guid.Parse("307fcf2d-696e-45ec-89fe-6db94e02e9e6"))
            {

            }
            var qi = new QueueItem(guid);
            var observable = dictionary.Get(qi, a => new Observable2<DateValue?>());
            Pipe.Instance.Queue(qi);
            return observable as Observable2<DateValue?>;
        }

        //public void Set(Guid guid)
        //{
        //    var qi = new QueueItem(guid);
        //    Pipe.Instance.Queue(qi);
        //}

        public override IObservable<Guid> Find(Guid guid, string name, System.Type? type = null, int? index = null)
        {
            var qi = new QueueItem(guid, name, type, index);
            var observable = dictionary.Get(qi, a => new Observable2<Guid>());
            Pipe.Instance.Queue(qi);
            return observable as Observable2<Guid>;
        }

        public override IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null)
        {

            var qi = new QueueItem(default, name, default, default, table_name, parentGuid);
            var observable = dictionary.Get(qi, a => new Observable2<IReadOnlyCollection<Key>>());
            Pipe.Instance.Queue(qi);
            return observable as Observable2<IReadOnlyCollection<Key>>;
        }

        public DecisionTree Predicate { get; set; }

        public void OnNext(QueueItem queueItem)
        {
            //if (item is TreeViewItem _item)
            //{
            Predicate.Reset();
            Predicate.Input = queueItem;
            Predicate.Evaluate();

            if (Predicate.Backput is string s)
            {
                var observable = dictionary[queueItem];

                if (s == "A")
                {
                    if (observable is Observable2<DateValue?> dv)
                        base.Get(queueItem.Guid).Subscribe(a => dv.OnNext(a));
                    else if (observable is Observable2<Guid> _dv)
                        base.Find(queueItem.Guid, queueItem.Name, queueItem.Type, queueItem.Index)
                            .Subscribe(a =>
                        {
                            _dv.OnNext(a);
                            _dv.OnCompleted();
                        });
                    else if (observable is Observable2<IReadOnlyCollection<Key>> cdv)
                        base.SelectKeys(queueItem.ParentGuid, queueItem.Name, queueItem.TableName)
                            .Subscribe(a =>
                        {
                            cdv.OnNext(a);
                            //cdv.OnCompleted();
                        });
                }
                else if (s == "B")
                {
                    if (observable is Observable2<IReadOnlyCollection<Key>> cdv)
                        base.SelectKeys()
                            .Subscribe(_keys =>
                            {
                                List<Key> keys = new();

                                foreach (var x in _keys)
                                {
                                    var table = new Table { Name = x.Name, Guid = x.Guid, Type = x.Instance.GetType() };
                                    keys.Add(x with { Instance = table });
                                }
                                cdv.OnNext(keys);
                                cdv.OnCompleted();
                            });
                    else
                        throw new Exception("DS 33 fff3");
                    //Observable2<Guid> observable = dictionary.Get(guid, a => new Observable2<Guid>()) as Observable2<Guid>;
                    //base.Find(guid).Subscribe(a => observable.OnNext(a));

                }
                //else if (s == "C")
                //{

                //    if (observable is Observable2<DateValue?> dv)
                //    {
                //        if (SelectedTable == null)
                //        {
                //            base.Get(queueItem.Guid).Subscribe(a => dv.OnNext(a));
                //        }
                //        else
                //        {
                //            dv.OnNext(new DateValue(DateTime.Now, SelectedTable));
                //        }
                //    }
                //    return;
                //}
                else if (s == "D")
                {
                    if (observable is Observable2<IReadOnlyCollection<Key>> cdv)
                    {
                        if (Locator.Current.GetService<Model>() is { SelectedTable:{ } selectedTable } )
                        {
                            cdv.OnNext(new List<Key>(new Key[] { new Key { Name = selectedTable.Name, Guid = selectedTable.Guid, Instance = Activator.CreateInstance(selectedTable.Type) } }));
                        }
                  
                    }
                    else
                        throw new Exception("DS 33 fff3");
                }
            }
            //}
            //return null;
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
