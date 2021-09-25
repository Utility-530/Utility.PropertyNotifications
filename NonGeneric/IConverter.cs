using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.NonGeneric
{
    public interface IConverter
    {
        object To(object value);
    }

    public interface ITwoWayConverter : IConverter
    {
        object From(object value);
    }
}
