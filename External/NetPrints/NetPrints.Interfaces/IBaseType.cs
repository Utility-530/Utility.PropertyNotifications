using NetPrints.Interfaces;
using System;
using System.ComponentModel;

namespace NetPrints.Graph
{
    public interface IBaseType : IName, IShortName, IFullCodeName, IFullCodeNameUnbound
    {
    }

    public interface IFullCodeNameUnbound
    {
        string FullCodeNameUnbound { get; }
    }

    public interface IShortName
    {
        string ShortName { get; }
    }

    public interface IValue
    {
        object Value { get; }
    }
}