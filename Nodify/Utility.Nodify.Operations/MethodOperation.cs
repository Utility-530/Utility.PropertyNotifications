using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodify.Base;
using Utility.Nodify.Core;

namespace Utility.Nodify.Operations
{
    public class MethodOperation(MethodInfo methodInfo) : IOperation, ISerialise, IGetValue, IIsReadOnly, IType
    {
        Lazy<Dictionary<string, ParameterInfo>> dictionary = new(() => methodInfo.GetParameters().ToDictionary(a => a.Name, a => a));
        public object Value => methodInfo.Name;
        bool IIsReadOnly.IsReadOnly => true;


        Type IType.Type => typeof(string);

        public IOValue Execute(params IOValue[] operands)
        {
            var parameters = operands.OrderBy(d => dictionary.Value.Keys.ToList().IndexOf(d.Title)).Select(a => a.Value).ToArray();
            var result = methodInfo.Invoke(null, parameters);
            return new IOValue(KeySource.Key(methodInfo, methodInfo.ReturnParameter), result);
        }

        public ISerialise FromString(string str)
        {
            var methodInfo = str.DeserialiseMethod();
            return new MethodOperation(methodInfo);
        }

        public override string ToString()
        {
            return methodInfo.Serialise();
        }
    }
}
