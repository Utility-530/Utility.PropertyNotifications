using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Interfaces.NonGeneric
{
    public interface IRemove
    {
        void Remove(object instance);  
    }  
    
    public interface IRemoveAt
    {
        void RemoveAt(int index);  
    }
}
