using System.Reflection;
using Utility.Interfaces.NonGeneric;
using System.Linq;
using System.Collections.Generic;

namespace Utility.Nodes.Meta
{
    internal static class DataActivator
    {
        public static object Activate(Structs.Repos.Key? a)
        {
            if (infos().SingleOrDefault() is { } constructorInfo)         
            {
                return constructorInfo.Invoke([a.Value.Name]);
            }

            var _data = ActivateAnything.Activate.New(a.Value.Type);

            if (_data is ISetName sname)
            {
                if (_data is IGetName { Name: { } name })
                {
                    if (name != a.Value.Name)
                        sname.Name = a.Value.Name;
                }
                else
                    sname.Name = a.Value.Name;
            }
            return _data;

            IEnumerable<ConstructorInfo> infos() => from x in a.Value.Type.GetConstructors()
                                                    let p = x.GetParameters()
                                                    where p.Length == 1 && p[0].ParameterType ==typeof(string)
                                                    select x;
        }
    }
}