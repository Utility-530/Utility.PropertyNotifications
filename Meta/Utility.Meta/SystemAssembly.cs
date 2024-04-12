using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Meta
{
    public class SystemAssembly : Assembly
    {
        const string System = "<System>";
        public override Type[] GetTypes() => Types;

        public static readonly Type[] Types = new[]{
                typeof(void),
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
                typeof(object),
        };

        public override string? FullName => System;

        public override AssemblyName GetName() => new (System);

        public override string ToString() => System;

    }
}
