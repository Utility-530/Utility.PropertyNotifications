using System;
using System.Collections;

namespace Utility.Interfaces.NonGeneric.Data
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
    
    public interface IUpdateManyBy
    {
        IEnumerable UpdateManyBy(IQuery query);
    }
}
