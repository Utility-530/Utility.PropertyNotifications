using System;
using System.Collections.Generic;
using System.Text;

namespace UtilityInterface.Database
{
    public interface IKey
    {
        string Key { get; set; }

    }


    public interface IDbRow
    {
        Int64 Id { get; set; }
        Int64 ParentId { get; set; }
    }


    public interface IDbTimeValue
    {
        Int64 Time { get; set; }
        Int64 Value { get; set; }
    }

    public interface IDbPeriod
    {

        Int64 Start { get; set; }
        Int64 End { get; set; }

    }

    public interface IDbRow<T>
    {
        Int64 Id { get; set; }
        Int64 ParentId { get; }
        T Parent { get; set; }

    }

}
