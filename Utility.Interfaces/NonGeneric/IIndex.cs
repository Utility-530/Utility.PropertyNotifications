using System;
using System.Collections.Generic;
using System.Text;

namespace Utility.Interfaces.NonGeneric
{
    public interface IGetIndex
    {
        int Index { get; }  
    }    
    
    public interface ISetIndex
    {
        int Index { set; }  
    }  
    
    public interface IIndex
    {
        int Index { get; set; }  
    }
}
