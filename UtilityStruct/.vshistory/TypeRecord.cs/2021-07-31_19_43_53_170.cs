using System;
using System.Collections.Generic;
using System.Text;
using UtilityStruct.Helper;

namespace UtilityStruct
{
    public ref struct TypeRecord
    {
        public TypeRecord(ReadOnlySpan<char> assembly, ReadOnlySpan<char> nameSpace, ReadOnlySpan<char> name)
        {
            Assembly = assembly;
            NameSpace = nameSpace;
            Name = name;
        }

        public ReadOnlySpan<char> Assembly { get; }

        public ReadOnlySpan<char> NameSpace { get; }

        public ReadOnlySpan<char> Name { get; }

        public Type ToType()
        {
            return TypeRecordHelper.ToType(this);
        }

        public static TypeRecord Create(Type type)
        {
            return TypeRecordHelper.AsRecord(type);
        }
    }

    public static class TypeRecordHelper
    {
        public static Type ToType(TypeRecord record)
        {

            return ToType(record.Assembly, record.NameSpace, record.Name);
        }

        public static Type ToType(ReadOnlySpan<char> assemblyName, ReadOnlySpan<char> nameSpace, ReadOnlySpan<char> name)
        {
            Type your = Type.GetType(SpanHelper.Concat(nameSpace, ".", name, ", ", assemblyName).ToString());
            return your;
        }

        public static TypeRecord AsRecord(this Type type)
        {
            if (type.Namespace == null || type.Assembly.FullName == null)
            {
                throw new Exception("sdfsdfsd  type");
            }
            return new TypeRecord(type.Assembly.FullName, type.Namespace, type.Name);
        }

    }
}
