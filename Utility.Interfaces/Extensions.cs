using System;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;

namespace Utility.Interfaces
{
    public static class Extensions
    {
        public static string Key(this IGetKey getKey) => getKey.Key;

        public static void SetKey(this ISetKey getKey, string key) => getKey.Key = key;

        public static T Parent<T>(this IGetParent<T> getKey) => getKey.Parent;

        public static void SetParent<T>(this ISetParent<T> setParent, T parent) => setParent.Parent = parent;

        public static object Data(this IGetData getKey) => getKey.Data;

        public static T Data<T>(this IGetData getKey) => (T)getKey.Data;

        public static void SetData(this ISetData setParent, object parent) => setParent.Data = parent;

        public static object Value(this IGetValue getKey) => getKey.Value;

        public static T Value<T>(this IGetValue getKey) => (T)getKey.Value;

        public static void SetValue(this ISetValue setParent, object parent) => setParent.Value = parent;

        public static object IsSelected(this IGetIsSelected getKey) => getKey.IsSelected;

        public static void SetIsSelected(this ISetIsSelected setParent, bool parent) => setParent.IsSelected = parent;

        public static Guid Guid(this IGetGuid getKey) => getKey.Guid;

        public static void SetGuid(this ISetGuid setParent, Guid parent) => setParent.Guid = parent;

        public static string Name(this IGetName getName)
        {      
                return getName.Name;
        }
        public static string SetName(this ISetName node, string name) => node.Name = name;

        public static System.Type toType(this object data)
        {
            return data is IGetType { } getType ? getType.GetType() : data.GetType();
        }
    }
}