using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Nodify.Base
{
    public static class KeySource
    {

        public static string Key(MethodInfo methodInfo, ParameterInfo parameterInfo)
        {
            return methodInfo.Name + "." + parameterInfo.Name;
        }
    }
}
