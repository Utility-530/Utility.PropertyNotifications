using System.Reflection;
using Utility.ProjectStructure;

namespace Utility.ProjectStructure
{
    public record AssemblyKeyValue(Assembly Assembly) : KeyValue(Assembly.FullName)
    {
        public override string GroupKey => nameof(Assembly);

        public override Assembly Value => Assembly;
    }
}

