using Splat;
using System;
using System.Collections.Generic;
using Utility.Helpers;
using IObserver = Utility.Models.IObserver;
using Utility.Repos;
using System.Linq;
using Utility.Trees.Decisions;
using Utility.Pipes;
using System.Linq.Expressions;
using Cogs.Collections;
using Utility.Extensions;

namespace Utility.Trees.Demo.Filters
{
    public class PipeRepository : TreeRepository
    {
        public class RepoDecisionTree : DecisionTreeX<RepoItem>
        {
            public RepoDecisionTree(Expression<Func<RepoItem, bool>> decision, string key, Func<RepoItem, object>? transform = null, Func<IEnumerable<object>, object?>? combine = null) :
                base(decision, key, transform, combine)
            {
            }
        }

        public class RepoXDecisionTree : DecisionTreeX<RepoItem>
        {
            private readonly TreeRepository repo;

            public RepoXDecisionTree(TreeRepository repo, Expression<Func<RepoItem, bool>> decision, string key, Func<RepoItem, object>? transform = null, Func<IEnumerable<object>, object?>? combine = null) :
                base(decision, key, transform, combine)
            {
                this.repo = repo;
            }

            public override void BackPropagate(List<object> keys)
            {
                base.BackPropagate(keys);
                if (IsBackputSet == false)
                    return;

                (RepoResult? ca, IDictionary<RepoItem, IObserver> dictionary, object input) = (Backput)this.Backput;

                int i = 0;
                var qi = ca.RepoItem;
                IDisposable? disposable = null;
                switch (ca.ResultType)
                {
                    case RepoResultType.Get when ca is RepoResult2X { Func: { } func }:
                        disposable = (func() as IObservable<DateValue>).Subscribe(_a => next(_a));
                        break;
                    case RepoResultType.Find when ca is RepoResult2X { Func: { } func }:
                        disposable = (func() as IObservable<Guid>).Subscribe(_a => next(_a));
                        break;
                    case RepoResultType.SelectKeys when ca is RepoResult2X { Func: { } func }:
                        disposable = (func() as IObservable<IReadOnlyCollection<Key>>).Subscribe(next);
                        break;
                    case RepoResultType.Special when ca is RepoResultX { Table: { } selectedTable }:
                        next(new List<Key>([new Key { Name = selectedTable.Name, /*Guid = selectedTable.Guid, */Instance = Activator.CreateInstance(selectedTable.Type) }]));
                        break;
                }
                void next(object? a)
                {
                    if (i++ > 0)
                    {

                    }
                    disposable?.Dispose();
                    if (input != (keys.First() as ForwardItem)?.Value)
                    {

                    }
                    var value = dictionary[input as RepoItem];

                    //var myKey = dictionary.FirstOrDefault(x => x.Key.Equals(input)).Key;
                    //var _value = dictionary.FirstOrDefault(x => x.Key.Equals(input)).Value;
                    value.OnNext(a);
                }
            }
        }

        public ObservableDictionary<RepoItem, IObserver> Dictionary { get; } = [];

        record Backput(RepoResult? Value, IDictionary<RepoItem, IObserver> Dictionary, object Input)
        {
        }

        protected PipeRepository(string? dbDirectory = null) : base(dbDirectory)
        {
            Predicate = new RepoXDecisionTree(this, item => (RepoItem)item != null,
                "a",
                combine:
                ca =>
                {
                    return new Backput(ca.FirstOrDefault(a => a != null) as RepoResult, Dictionary, Predicate.Input);
                })
            {
                new RepoDecisionTree(item => item.Name == "SelectedTable" && item.ItemType == RepoItemType.SelectKeys,
                "b.1",
                cdv =>
                {
                    if (Locator.Current.GetService<Model>() is { SelectedTable:{ } selectedTable } )
                    {
                        //return Obs.Return(new List<Key>([new Key { Name = selectedTable.Name, Guid = selectedTable.Guid, Instance = Activator.CreateInstance(selectedTable.Type) }]));
                        return new RepoResultX(cdv,  RepoResultType.Special, selectedTable);
                    }
                    return new RepoResultX(cdv,  RepoResultType.Special, null);
                }),
                new RepoDecisionTree(
                    item => true,
                    "b.2")
                {
                    new RepoDecisionTree(item => item.ItemType == RepoItemType.Get, "c.1", qi=> new RepoResult2X(qi,()=> base.Get(qi.Guid), RepoResultType.Get)),
                    new RepoDecisionTree(item => item.ItemType == RepoItemType.Find, "c.2", qi=> new RepoResult2X(qi,()=> base.Find(qi.Guid, qi.Name, qi.Type, qi.Index), RepoResultType.Find)),
                    new RepoDecisionTree(item =>  item.ItemType == RepoItemType.SelectKeys, "c.3", qi=> new RepoResult2X(qi,()=> base.SelectKeys(qi.ParentGuid, qi.Name, qi.TableName), RepoResultType.SelectKeys)),
                },
            };
        }

        public override IObservable<DateValue> Get(Guid guid)
        {
            var qi = new RepoItem(guid, RepoItemType.Get);
            Pipe.Instance.New(new ForwardItem(Predicate, qi, []));
            if (Dictionary.ContainsKey(qi))
            {

            }
            return Dictionary.Get(qi, a => new Subject<DateValue>()) as IObservable<DateValue>;
        }

        public override IObservable<Guid> Find(Guid guid, string name, System.Type? type = null, int? index = null)
        {
            var qi = new RepoItem(guid, RepoItemType.Find, name, type, index);
            Pipe.Instance.New(new ForwardItem(Predicate, qi, []));
            if (Dictionary.ContainsKey(qi))
            {

            }
            return Dictionary.Get(qi, a => new Subject<Guid>()) as Subject<Guid>; ;
        }

        public override IObservable<IReadOnlyCollection<Key>> SelectKeys(Guid? parentGuid = null, string? name = null, string? table_name = null)
        {
            var qi = new RepoItem(default, RepoItemType.SelectKeys, name, default, default, table_name, parentGuid);
            Pipe.Instance.New(new ForwardItem(Predicate, qi, []));
            if (Dictionary.ContainsKey(qi))
            {

            }
            return Dictionary.Get(qi, a => new Subject<IReadOnlyCollection<Key>>()) as Subject<IReadOnlyCollection<Key>>;
        }

        public IDecisionTreeX Predicate { get; set; }

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
