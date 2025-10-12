using System;
using Utility.Enums;

namespace Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataTypeAttribute(DataType dataType) : Attribute
    {
        public DataType DataType { get; } = dataType;
    }
}

