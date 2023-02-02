using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Common.Contract;

public record CollectionChange
{
    public NotifyCollectionChangedAction Action { get; }
    public IEnumerable<object> Items { get; }

    public CollectionChange(NotifyCollectionChangedAction action, IEnumerable<object> item2)
    {
        Action = action;
        Items = item2;
    }

    public void Deconstruct(out NotifyCollectionChangedAction action, out IEnumerable<object> item)
    {
        action = Action;
        item = Items;
    }

    public static implicit operator (NotifyCollectionChangedAction Action, IEnumerable<object>)(CollectionChange value)
    {
        return (value.Action, value.Items);
    }

    public static implicit operator CollectionChange((NotifyCollectionChangedAction, IEnumerable<object>) value)
    {
        return new CollectionChange(value.Item1, value.Item2);
    }
}

public record CollectionChangeMessage(CollectionChange change);

public record RepositoryMessage(IRepository Service);

public interface ICollectionService : IObserver<RepositoryMessage>, IObservable<CollectionChangeMessage>
{
    ObservableCollection<object> Items { get; }
}