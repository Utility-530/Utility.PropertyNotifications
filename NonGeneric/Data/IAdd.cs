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
        IEnumerable AddMany(IEnumerable items);
    }   
    public interface IAddManyBy
    {
        IEnumerable AddManyBy(IQuery query);
    }
}
