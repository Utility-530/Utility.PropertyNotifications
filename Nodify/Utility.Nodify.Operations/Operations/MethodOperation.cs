using System;
using System.Linq;
using System.Reflection;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Base;

namespace Utility.Nodify.Operations.Operations
{
    public class MethodOperation : IOperation, ISerialise, IValue, IIsReadOnly, IType
    {
        private readonly MethodInfo methodInfo;

        public MethodOperation(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }

        public object Value => methodInfo.Name;
        bool IIsReadOnly.IsReadOnly => true;


        Type IType.Type => typeof(string);

        public IOValue[] Execute(params IOValue[] operands)
        {
            var parameters = methodInfo.GetParameters().Select(a => a.Name).ToList();
            operands.OrderBy(d => parameters.IndexOf(d.Title)).ToList();
            var @return = new IOValue(KeySource.Key(methodInfo, methodInfo.ReturnParameter), methodInfo.Invoke(null, operands.Select(a => a.Value).ToArray()));
            return new[] { @return };
        }

        public ISerialise FromString(string str)
        {
            var methodInfo = TypeHelper.DeserialiseMethod(str);
            return new MethodOperation(methodInfo);
        }

        public override string ToString()
        {
            return methodInfo.Serialise();
        }
    }
}
