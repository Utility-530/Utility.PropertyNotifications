using Utility.Interfaces.NonGeneric.Data;

namespace Utility.Persists.Infrastructure
{
    public class MaxRowId : IQuery
    {
    }

    public class MaxRowIdResult : IQueryResult
    {

        public MaxRowIdResult(int id, bool isSuccess)
        {
            Id = id;
            IsSuccess = isSuccess;
        }
        public int Id { get; }

        public bool IsSuccess { get; }
    }

}
