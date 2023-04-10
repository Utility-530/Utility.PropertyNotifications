using System;

namespace Utility.Common
{
    public class TypeObject
    {
        public string? TypeName { get; init; }
        public string? Key { get; init; }
        public object? Object { get; init; }
        public Type? Type { get; init; }
    }
}