using System.Linq;
using System.Reflection;
using Utility.Enums;

namespace Utility.WPF.Meta
{
    public class AssemblyKeyValue : KeyValue
    {
        private readonly Assembly assembly;
        public AssemblyKeyValue(Assembly assembly, AssemblyType categoryKey) : base(assembly.FullName.Split(",").First())
        {
            this.assembly = assembly;
            CategoryKey = categoryKey;
        }

        public override Assembly Value => assembly;

        public override string? GroupKey => Key?.Split(".").First();

        public virtual AssemblyType CategoryKey { get; }

        public override string ToString()
        {
            return Key ?? "No Key!";
        }
    }
}