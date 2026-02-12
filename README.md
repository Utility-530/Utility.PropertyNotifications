
# Utility Interfaces
![.NET 2](https://img.shields.io/badge/.NET-2.0-blue)
[![NuGet version (Newtonsoft.Json.Bson)](https://img.shields.io/nuget/v/Utility.Interfaces)](https://www.nuget.org/packages/Utility.Interfaces/)

A collection of interface types (generally) with a single readonly property
e.g

    public interface ICount
    {
        int Count { get; }
    }

or method

    public interface ICopy
    {
        string Copy();
    }

and names, which match their properties

some types are composed of get and set sub-types

e.g

    public interface IData : IGetData, ISetData
    {
    }

    public interface IGetData
    {
        object Data { get; }
    }

    public interface ISetData
    {
        object Data { set; }
    }
