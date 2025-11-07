using System.Collections.Generic;

namespace Utility.Interfaces.Generic.Data
{
    public interface IRemove<T, TQueryResult>
    {
        TQueryResult Remove(T item);
    }

    public interface IRemoveBy<TQuery, TQueryResult>
    {
        TQueryResult RemoveBy(TQuery query);
    }

    public interface IRemoveById<TId, TQueryResult>
    {
        TQueryResult RemoveById(TId id);
    }

    public interface IRemoveMany<T, TQueryResult>
    {
        TQueryResult RemoveMany(IEnumerable<T> items);
    }

    public interface IRemoveManyBy<TQuery, TQueryResult>
    {
        TQueryResult RemoveManyBy(TQuery query);
    }

    public interface IRemoveManyById<TId, TQueryResult>
    {
        TQueryResult RemoveManyById(IEnumerable<TId> ids);
    }
}