using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UtilityAttribute
{

    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class EnumAttribute : Attribute
    {
        public byte Enum;
        public Type Type;

        public EnumAttribute(byte @enum, Type type)
        {
            Enum = @enum;
            Type = type;
        }
    }


    public static class EnumAttributeHelper
    {
        public static MethodInfo GetMethodByEnum(byte @enum, Type type, Type @class)
        {
            var attribute = typeof(EnumAttribute).GetField(nameof(EnumAttribute.Enum));
            return GetMethodsByAttribute(@class, type)
                 .Single(_ =>
                 {
                     var customattribute = _.GetCustomAttribute(typeof(EnumAttribute));
                     return (byte)attribute.GetValue(customattribute) == @enum;
                 });
        }



        static IEnumerable<MethodInfo> GetMethodsByAttribute(Type t, Type attribute) =>
            t.GetMethods().Where(
                    method => Attribute.IsDefined(method, attribute));
    }
}
