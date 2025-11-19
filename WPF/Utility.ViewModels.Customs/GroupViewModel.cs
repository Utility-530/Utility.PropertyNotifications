using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;

//using Utility.Common.Models;
using Utility.Helpers;
using Utility.Models;

namespace Utility.ViewModels;

//public class GroupViewModel : ViewModel
//{
//    public GroupViewModel(string key) : base(key)
//    {
//    }
//}

public class GroupViewModel<T, TKey, TGroupKey> : ViewModel
{
    private int count;
    private IReadOnlyCollection<T> collection;

    public GroupViewModel(IGroup<T, TKey, TGroupKey> group) : base(group.Key.ToString())
    {
        group
             .Cache
             .Connect()
             .ToCollection()
             .Subscribe(a =>
             {
                 this.RaiseAndSetIfChanged(ref count, a.Count, nameof(Count));
                 this.RaiseAndSetIfChanged(ref collection, a, nameof(Children));
             },
             e => { })
            .DisposeWith(disposer);
    }

    public override int Count => count;

    public override IReadOnlyCollection<T> Children => collection;

    public override Property Model => throw new NotImplementedException();

    //public override Model Model { get; }
}

public class GroupViewModel<T, TGroupKey> : ViewModel
{
    private readonly IGroup<T, TGroupKey> group;
    private int count;
    private IReadOnlyCollection<T> collection;

    public GroupViewModel(IGroup<T, TGroupKey> group) : base(group.GroupKey.ToString())
    {
        group
             .List
             .Connect()
             .ToCollection()
            .Subscribe(a =>
            {
                this.RaiseAndSetIfChanged(ref count, a.Count, nameof(Count));
                this.RaiseAndSetIfChanged(ref collection, a, nameof(Children));
            },
            e =>
            {
            })
            .DisposeWith(disposer);
        this.group = group;
    }

    public override string Key => group.GroupKey.ToString();

    public override int Count => count;

    public override IReadOnlyCollection<T> Children => collection;

    public override Property Model => throw new NotImplementedException();

    //public override Property Model { get; }
}

public class Group<T, TKey, TGroupKey> : IGroup<T, TKey, TGroupKey>
{
    public Group(TGroupKey key, IObservable<IChangeSet<T, TKey>> observable)
    {
        Cache = observable.AsObservableCache();
        Key = key;
    }

    public IObservableCache<T, TKey> Cache { get; }

    public TGroupKey Key { get; }
}

public class Group<T, TGroupKey> : IGroup<T, TGroupKey>
{
    public Group(TGroupKey key, IEnumerable<T> observable)
    {
        List = observable.AsObservableChangeSet().AsObservableList();
        GroupKey = key;
    }

    public TGroupKey GroupKey { get; }
    public IObservableList<T> List { get; }
}