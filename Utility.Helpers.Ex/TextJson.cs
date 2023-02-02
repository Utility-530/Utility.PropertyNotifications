using System;
using System.Text.Json;

namespace Utility.Helpers.Ex
{
    public static class TextJsonHelper
    {
        static JsonSerializerOptions options = new();

        public static object? Deserialise(string str, Type type)
        {
            return JsonSerializer.Deserialize(str, type, options);
        }
        public static string Serialise(object obj, Type type)
        {
            return JsonSerializer.Serialize(obj, type, options);
        }

        public static object? Deserialise<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str, options);
        }
        public static string Serialise<T>(object obj)
        {
            return JsonSerializer.Serialize(obj, typeof(T), options);
        }


        public static bool Compare<T>(T item1, T item2)
        {
            var type = typeof(T);
            var string1 = JsonSerializer.Serialize(item1, type, options);
            var string2 = JsonSerializer.Serialize(item2, type, options);
            return string1.Equals(string2);
        }
    }
}