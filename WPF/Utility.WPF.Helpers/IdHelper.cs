using System;
using System.Linq;

namespace Utility.WPF.Helpers
{
    // Also in Utility.WPF.CommonCore in Reflection.cs
    public static class IdHelper
    {
        private static readonly string[] IdNames = { "id", "key", "name", "identification" };

        public static string? GetIdProperty<T>() => GetIdProperty(typeof(T));

        public static string? GetIdProperty(Type type)
        {
            var @select = type.GetProperties().Select(a => a.Name);
            return @select.FirstOrDefault(name => IdNames.Contains(name.ToLower()));
        }

        public static bool CheckIdProperty(string id, Type type)
        {
            var pt = type.GetProperty(id)?.PropertyType ?? throw new Exception("SDDf4 ggg5");
            var interfaces = pt.GetInterfaces();
            return type.IsAssignableFrom(typeof(IConvertible)) ||
                   interfaces
                       .Select(a => a.Name)
                       .Any(a => a.StartsWith("IEquatable"));
        }
    }
}