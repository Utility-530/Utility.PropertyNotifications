using System;
using System.Collections;

namespace UtilityInterface.NonGeneric.Data
{
    public interface IUpdate
    {
        object Update(object item);

    }

    public interface IUpdateBy
    {
        object UpdateBy(IQuery query);
    }

    public interface IUpdateMany
    {
        IEnumerable UpdateMany(IEnumerable items);
    }
}
