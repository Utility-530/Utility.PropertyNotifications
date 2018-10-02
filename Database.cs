using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.Database
{
    public interface IKey
    {
        String Key { get; set; }

    }

    public interface IId
    {
        Int64 Id { get; set; }
    }


    public interface IChildRow: IId
    {
        Int64 ParentId { get; set; }
    }


    public interface IChildRow<T> : IChildRow where T: IId
    {
        T Parent { get; set; }
    }


    public interface ITimeValue
    {
        Int64 Time { get; set; }
        Int64 Value { get; set; }
    }

    public interface IPeriod
    {

        Int64 Start { get; set; }
        Int64 End { get; set; }
        
    }

}
