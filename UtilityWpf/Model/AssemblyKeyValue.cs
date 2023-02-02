using System.Linq;
using System.Reflection;
using Utility.Enums;

namespace UtilityWpf.Model
{
    public class AssemblyKeyValue : KeyValue
    {
        private readonly Assembly assembly;
        public AssemblyKeyValue(Assembly assembly, AssemblyType categoryKey)
        {
            this.assembly = assembly;
            CategoryKey = categoryKey;
        }

        public override string? Key => assembly.FullName.Split(",").First();

        public override Assembly Value => assembly;

        public override string? GroupKey => Key?.Split(".").First();

        public virtual AssemblyType CategoryKey { get; }

        public override string ToString()
        {
            return Key ?? "No Key!";
        }
    }
}