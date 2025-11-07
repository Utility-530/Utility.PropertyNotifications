using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;

namespace Utility.ProjectStructure
{
    public record AssemblyKey(Assembly Assembly) : StringKey(Assembly.FullName), IGetName
    {
        public string GroupKey => nameof(Assembly);

        public string Name => Assembly.GetName().Name;

        public override string ToString()
        {
            return Assembly.FullName;
        }
    }
}