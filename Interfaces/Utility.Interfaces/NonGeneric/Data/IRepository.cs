using System;


namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IRepository : IBasicRepository, IQuerySingleRepository, IQueryMultiRepository
    {
    }

    public interface IBasicRepository: ISingleRepository, IMultiRepository
    {
    }

    public interface IIdRepository : IIdSingleRepository, IIdMultiRepository
    {
    }

    public interface ISingleRepository : IFind, IAdd, IUpdate, IRemove
    {
    }

    public interface IMultiRepository : IFindMany, IAddMany, IUpdateMany, IRemoveMany
    {
    }

    public interface IQuerySingleRepository: IFindBy, IAddBy, IUpdateBy, IRemoveBy
    {
    }
    public interface IQueryMultiRepository : IFindManyBy, IAddManyBy, IUpdateManyBy, IRemoveManyBy
    {
    }

    public interface IIdSingleRepository : IFindById, IRemoveById
    {
    }

    public interface IIdMultiRepository: IFindManyById, IRemoveManyById
    {
    }
}
