using System;
using System.Collections;
using System.Collections.Generic;

namespace UtilityInterface.NonGeneric.Data
{
    public interface IRemove
    {
        object Remove(object item);
    }

    public interface IRemoveBy
    {
        object RemoveBy(IQuery query);
    }

    public interface IRemoveById
    {
        object RemoveById(long id);
    }

    public interface IRemoveByGuid
    {
        object RemoveByGuId(Guid guid);
    }

    public interface IRemoveMany
    {
        IEnumerable RemoveMany(IEnumerable items);
    } 

    public interface IRemoveManyBy
    {
        IEnumerable RemoveMany(IQuery query);
    } 
    
    public interface IRemoveManyById
    {
        IEnumerable RemoveManyById(IEnumerable<long> ids);
    }

    public interface IRemoveManyByGuid
    {
        IEnumerable RemoveManyById(IEnumerable<Guid> guids);
    }

}
