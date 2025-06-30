using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models
{
    public class Method
    {
        public Method(MethodInfo methodInfo, object instance)
        {
            MethodInfo = methodInfo;
            Instance = instance;
        }

        public MethodInfo MethodInfo { get; }
        public object Instance { get; }
        public string Name => MethodInfo.Name;

        public object? Execute(params object?[] objects)
        {
            return MethodInfo.Invoke(Instance, objects);
        }

    }
}
