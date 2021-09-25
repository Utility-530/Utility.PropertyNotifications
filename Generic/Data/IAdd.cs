using System;
using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
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
}
