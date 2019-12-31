using System;
using System.Collections.Generic;
using System.Text;
using UtilityInterface.NonGeneric.Database;

namespace UtilityInterface.Generic.Database
{


    public interface ISelectAll<T>
    {
        IEnumerable<T> SelectAll();
    }


    public interface ISelect<T>
    {
        T Select(T item);
    }

    public interface ISelectById<T,R>
    {
        T SelectById(R id);
    }

    public interface ISelectBy<T>
    {
        IEnumerable<T> SelectBy(Func<T,bool> id);
    }


    public interface IInsert<T>
    {
        bool Insert(T item);
    }

    public interface IInsertBulk<T>
    {
        int InsertBulk(IEnumerable<T> item);
    }


    public interface IUpdate<T>
    {
        bool Update(T item);

    }

    public interface IUpdateBulk<T>
    {
        int UpdateBulk(IEnumerable<T> item);
    }

    public interface IDelete<T>
    {
        bool Delete(T item);
    }

    public interface IDeleteBulk<T>
    {
        int DeleteBulk(IEnumerable<T> item);
    }

    public interface IDeleteById<R>
    {
        bool DeleteById(R id);
    }


    public interface IDbService<T,R> : IDisposable, ISelectAll<T>, ISelect<T>, ISelectById<T,R>, IInsert<T>, IInsertBulk<T>, IUpdate<T>, IUpdateBulk<T>, IDelete<T>, IDeleteBulk<T>, IDeleteById<R>
    {

    }

    public interface IId<T>
    {
        T Id { get; set; }
    }

    public interface IChildRow<T> : IChildRow where T: IId
    {
        T Parent { get; set; }
    }


    public interface IValue<T>
    {
        T Value { get; set; }
    }

    public interface ITimeValue<T> : ITime, IValue<T>
    {
    }

}
