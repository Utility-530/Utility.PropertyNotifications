using System;
using System.Collections.Generic;
using System.Text;
using UtilityInterface.NonGeneric;

namespace UtilityInterface.Generic
{
    public interface IConverter<TIn, TOut>
    {
        TOut To(TIn value);
    }

    public interface ITwoWayConverter<TIn, TOut> : IConverter<TIn, TOut>
    {
        TIn From(TOut value);
    }
}
