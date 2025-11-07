using System;
using System.Collections;
using System.Collections.Generic;

namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IFind
    {
        object Find(object item);
    }

    public interface IFindBy
    {
        object FindBy(IQuery query);
    }

    public interface IFindById
    {
        object FindById(long id);
    }

    public interface IFindByGuid
    {
        object FindByGuid(Guid guid);
    }

    public interface IFindMany
    {
        IEnumerable FindMany(IEnumerable items);
    }

    public interface IFindManyBy
    {
        IEnumerable FindManyBy(IQuery query);
    }

    public interface IFindManyById
    {
        IEnumerable FindManyById(IEnumerable<long> ids);
    }

    public interface IFindManyByGuid
    {
        IEnumerable FindManyByGuid(IEnumerable<Guid> guids);
    }
}