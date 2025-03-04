using System.Reflection;
using Utility.Interfaces.NonGeneric;
using System.Linq;

namespace Utility.Nodes.Filters
{
    internal static class DataActivator
    {

        public static object Activate(Structs.Repos.Key? a)
        {
            if (a.Value.Type.GetConstructors().SingleOrDefault(a => a.GetParameters().SingleOrDefault(a => a.ParameterType == typeof(string)) is not null) is ConstructorInfo constructorInfo)
            {
                return constructorInfo.Invoke([a.Value.Name]);
            }

            var _data = ActivateAnything.Activate.New(a.Value.Type);

            if (_data is ISetName sname)
            {
                sname.Name = a.Value.Name;
            }
            return _data;
        }
    }
}