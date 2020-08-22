using System;

namespace UtilityInterface.Generic.Database
{
    public interface IDatabaseService<T, R> : IDisposable, ISelectAll<T>, ISelect<T>, ISelectById<T, R>, IInsert<T>, IInsertBulk<T>, IUpdate<T>, IUpdateBulk<T>, IDelete<T>, IDeleteBulk<T>, IDeleteById<R>
    {

    }

}
