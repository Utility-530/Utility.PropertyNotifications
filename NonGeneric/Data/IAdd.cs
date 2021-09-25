using System;
using System.Collections;
using System.Collections.Generic;

namespace UtilityInterface.NonGeneric.Data
{
    public interface IAdd
    {
        object Add(object item);
    }  
    

    public interface IAddBy
    {
        object AddBy(IQuery query);
    }

    public interface IAddMany
    {
        IEnumerable IAddMany(IEnumerable items);
    }
}
