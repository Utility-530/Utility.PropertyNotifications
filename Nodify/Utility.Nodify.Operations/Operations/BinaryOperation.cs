using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Utility.Interfaces.NonGeneric;
using static Utility.Conversions.ConversionHelper;

namespace Utility.Nodify.Operations
{
    public class BinaryOperation : IOperation, ISerialise
    {
        private readonly Func<double, double, double> _func;

        public BinaryOperation(Func<double, double, double> func) => _func = func;

        public IOValue[] Execute(params IOValue[] operands)
            => new[] { new IOValue(default, _func.Invoke(ChangeType<double>(operands[0].Value), (double)ChangeType<double>(operands[1].Value))) };

        public ISerialise FromString(string str)
        {
            BinaryFormatter formatter = new();
            byte[] bytes = Convert.FromBase64String(str);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                var x = formatter.Deserialize(stream);
                return new BinaryOperation((Func<double, double, double>)x);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }

        public override string ToString()
        {
            BinaryFormatter formatter = new();
            var memoryStream = new MemoryStream();
            using (memoryStream)
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                formatter.Serialize(memoryStream, this._func);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }


    //[Serializable]
    //public struct ActionInfo
    //{
    //    [Serializable]
    //    public struct TypeInfo
    //    {
    //        public string Assembly { get; }
    //        public string Class { get; }

    //        public TypeInfo(Type type)
    //        {
    //            Assembly = type.Assembly.FullName;
    //            Class = type.FullName;
    //        }

    //        public Type Load()
    //        {
    //            var assembly = AppDomain.CurrentDomain.Load(Assembly);
    //            return assembly.GetType(Class);
    //        }
    //    }

    //    public TypeInfo ActionType { get; }
    //    public string ActionName { get; }
    //    public TypeInfo[] ActionParameters { get; }
    //    public BindingFlags ActionBindingFlags { get; }
    //    public TypeInfo DelegateType { get; }

    //    public ActionInfo(Delegate action)
    //    {
    //        var method = action.Method;

    //        if (!method.IsStatic)
    //            throw new ArgumentException("Action must be a static method");

    //        ActionType = new(method.DeclaringType);
    //        ActionName = method.Name;
    //        ActionParameters = method.GetParameters().Select(parameter => new TypeInfo(parameter.ParameterType)).ToArray();
    //        ActionBindingFlags = BindingFlags.Static | (method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic);

    //    }
    //}
}