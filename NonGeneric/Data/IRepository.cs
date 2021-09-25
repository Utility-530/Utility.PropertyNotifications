using System;


namespace UtilityInterface.NonGeneric.Data
{
    public interface IRepository : IBasicRepository, IQueryRepository
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

    public interface IQueryRepository: IFindBy, IAddBy, IUpdateBy, IRemoveBy
    {
    }

    public interface IIdSingleRepository : IFindById, IRemoveById
    {
    }

    public interface IIdMultiRepository: IFindManyById, IRemoveManyById
    {
    }
}
