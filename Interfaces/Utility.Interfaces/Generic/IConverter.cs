namespace Utility.Interfaces.Generic
{

    public interface IConverter
    {
    }

    public interface IConverter<TIn, TOut> : IConverter
    {
        TOut Convert(TIn value);
    }

    public interface ITwoWayConverter<TIn, TOut> : IConverter<TIn, TOut>
    {
        TIn ConvertBack(TOut value);
    }
}
