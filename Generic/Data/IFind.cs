using System.Collections.Generic;

namespace UtilityInterface.Generic.Database
{
    public interface IFind<T, TQueryResult>
    {
        TQueryResult Find(T item);
    }

    public interface IFindMany<T, TQueryResult> 
    {
        TQueryResult FindMany(IEnumerable<T> items);
    }

    public interface IFindBy<TQuery, TQueryResult>
    {
        TQueryResult FindBy(TQuery items);
    }

    public interface IFindManyBy<TQuery, TQueryResult> 
    {
        TQueryResult FindManyBy(TQuery items);
    }


    public interface IFindById<TId, TQueryResult>
    {
        TQueryResult FindById(TId id);
    }

    public interface IFindManyById<TId, TQueryResult>
    {
        TQueryResult FindManyById(IEnumerable<TId> items);
    }

}
