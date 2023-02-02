namespace Utility.Interfaces.Generic
{
    public interface IMapper<TIn, TOut>
    {
        TOut To(TIn value);
    }

    public interface ITwoWayMapper<TIn, TOut> : IConverter<TIn, TOut>
    {
        TIn From(TOut value);
    }
}
