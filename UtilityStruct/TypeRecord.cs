using System;

namespace UtilityStruct
{
    public ref struct TypeRecord
    {
        public TypeRecord(ReadOnlySpan<char> assembly, ReadOnlySpan<char> nameSpace, ReadOnlySpan<char> name)
        {
            Assembly = assembly;
            Namespace = nameSpace;
            Name = name;
        }

        public ReadOnlySpan<char> Assembly { get; }

        public ReadOnlySpan<char> Namespace { get; }

        public ReadOnlySpan<char> Name { get; }

        public Type? ToType()
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
        public static Type? ToType(TypeRecord record)
        {
            return ToType(record.Assembly, record.Namespace, record.Name);
        }

        public static Type? ToType(ReadOnlySpan<char> assemblyName, ReadOnlySpan<char> nameSpace, ReadOnlySpan<char> typeName)
        {
            var resultSpan = new char[assemblyName.Length + nameSpace.Length + typeName.Length + 3].AsSpan();
            // start with the type-name (including name-space)
            nameSpace.CopyTo(resultSpan);
            int from = nameSpace.Length;
            resultSpan[from] = '.';
            from++;
            typeName.CopyTo(resultSpan[from..]);
            from += typeName.Length;

            // separate
            resultSpan[from] = ',';
            from++;
            resultSpan[from] = ' ';
            from++;

            // append assembly-name
            assemblyName.CopyTo(resultSpan[from..]);
            Type? your = Type.GetType(resultSpan.ToString());
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