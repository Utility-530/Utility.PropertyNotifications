using System.Linq;
using System.Reflection;
using Utility.Enums;

namespace Utility.WPF.Meta
{
    public record AssemblyKeyValue(Assembly Assembly, AssemblyType CategoryKey)  : KeyValue(Assembly.FullName.Split(",").First())
    {

        public override Assembly Value => Assembly;

        public override string? GroupKey => Key?.Split(".").First();

        public virtual AssemblyType CategoryKey { get; }

        public override string ToString() => Key ?? "No Key!";
    }
}