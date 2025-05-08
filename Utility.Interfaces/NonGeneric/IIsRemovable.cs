using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.NonGeneric
{
    public interface IIsRemovable
    {
        bool IsRemovable { get; set; }
    }  
      
    public interface IGetIsRemovable
    {
        bool IsRemovable { get; }
    }  
    
    public interface ISetIsRemovable
    {
        bool IsRemovable { set; }
    }
}
