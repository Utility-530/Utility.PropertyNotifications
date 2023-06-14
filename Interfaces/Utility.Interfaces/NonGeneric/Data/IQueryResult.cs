
namespace Utility.Interfaces.NonGeneric.Data
{
    public interface IQueryResult : IResult
    {

    }

    public interface IResult
    {
        bool IsSuccess { get; }
    }
}
