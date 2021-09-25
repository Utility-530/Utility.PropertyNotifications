using System;
using System.Collections.Generic;

namespace UtilityInterface.Generic.Data
{
    public interface IUpdate<T, TQueryResult>
    {
        TQueryResult Update(T item);
    }

    public interface IUpdateMany<T, TQueryResult>
    {
        TQueryResult UpdateMany(IEnumerable<T> items);
    }  
    
    public interface IUpdateBy<TQuery, TQueryResult>
    {
        TQueryResult UpdateBy(TQuery query);
    }

    public interface IUpdateManyBy<TQuery, TQueryResult>
    {
        TQueryResult UpdateManyBy(TQuery query);
    }

}
