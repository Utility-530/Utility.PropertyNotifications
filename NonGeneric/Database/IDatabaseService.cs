using System;

namespace UtilityInterface.NonGeneric.Database
{
    public interface IDatabaseService : IDisposable, ISelectAll, ISelect, ISelectById, IInsert, IInsertBulk, IUpdate, IUpdateBulk, IDelete, IDeleteBulk, IDeleteById
    {

    }
}
