using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.NonGeneric.Database
{



    public interface ISelectAll
    {
        IEnumerable SelectAll();
    }


    public interface ISelect
    {
        object Select(object item);
    }

    public interface ISelectById
    {
        object SelectById(object item);
    }
    public interface IInsert
    {
        bool Insert(object item);
    }

    public interface IInsertBulk
    {
        int InsertBulk(IEnumerable item);
    }


    public interface IUpdate
    {
        bool Update(object item);

    }

    public interface IUpdateBulk
    {
        int UpdateBulk(IEnumerable item);
    }

    public interface IDelete
    {
        bool Delete(object item);
    }

    public interface IDeleteBulk
    {
        int DeleteBulk(IEnumerable item);
    }

    public interface IDeleteById
    {
        bool DeleteById(object item);
    }


    public interface IDbService : IDisposable, ISelectAll, ISelect, ISelectById, IInsert, IInsertBulk, IUpdate, IUpdateBulk, IDelete, IDeleteBulk, IDeleteById
    {

    }


    public interface IId
    {
        long Id { get; }
    }

    public interface IGuid
    {
        Guid Guid { get; }
    }



    public interface IChildRow : IId
    {
        long ParentId { get; }
    }
}
