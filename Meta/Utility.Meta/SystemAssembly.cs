using System.Reflection;

namespace Utility.Meta
{
    public class SystemAssembly : Assembly
    {
        private const string System = "<System>";

        public override Type[] GetTypes() => Types;

        public static readonly Type[] Types = new[]{
                //typeof(void),
                typeof(string),
                typeof(Enum),
                typeof(bool),
                typeof(int),
                typeof(short),
                typeof(long),
                typeof(double),
                typeof(byte),
                typeof(Guid),
                typeof(DateTime),
                typeof(Type),
                typeof(object),
        };

        public override string? FullName => System;

        public override AssemblyName GetName() => new(System);

        public override string ToString() => System;
    }

    public class ReflectionAssembly : Assembly
    {
        private const string Reflection = "<Reflection>";

        public override Type[] GetTypes() => Types;

        public static readonly Type[] Types = new[]{
                //typeof(void),
                typeof(Assembly),
                typeof(Type),
                typeof(PropertyInfo),              
        };

        public override string? FullName => Reflection;

        public override AssemblyName GetName() => new(Reflection);

        public override string ToString() => Reflection;
    }
}