using System;
using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface IUpdate<T, TQueryResult>
    {
        TQueryResult Update(T item);
    }

    public interface IUpdateMany<T, TQueryResult>
    {
        TQueryResult UpdateMany(IEnumerable<T> item);
    }  
    
    public interface IUpdateBy<TQuery, TQueryResult>
    {
        TQueryResult UpdateBy(TQuery predicate);
    }

}
