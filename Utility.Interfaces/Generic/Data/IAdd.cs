using System;
using System.Collections.Generic;

namespace Utility.Interfaces.Generic.Data
{
    public interface IAdd<T, TQueryResult>
    {
        TQueryResult Add(T item);
    }
    public interface IAddMany<T, TQueryResult>
    {
        TQueryResult AddMany(IEnumerable<T> items);
    }  

    public interface IAddBy<TQuery, TQueryResult>
    {
        TQueryResult AddBy(TQuery query);
    }

    public interface IAddManyBy<TQuery, TQueryResult>
    {
        TQueryResult AddManyBy(TQuery query);
    }
}
