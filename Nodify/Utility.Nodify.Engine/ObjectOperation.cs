using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Meta;
using Utility.Nodify.Core;
using Utility.Nodify.Operations.Infrastructure;
using Utility.PropertyDescriptors;
using MemberDescriptor = Utility.PropertyDescriptors.MemberDescriptor;


namespace Utility.Nodify.Operations.Operations
{
    public class ObjectOperation : IOperation, ISerialise, IIsReadOnly, IValue, IType
    {
        private readonly ObjectInfo objectInfo;
        private MemberDescriptor rootDescriptor;

        public ObjectOperation(ObjectInfo objectInfo)
        {
            this.objectInfo = objectInfo;
        }

        public object Value => objectInfo.Name;

        bool IIsReadOnly.IsReadOnly => true;

        Type IType.Type => typeof(string);

        public IOValue Execute(params IOValue[] operands)
        {
            if (operands.Any() == false)
            {
                //var instance = DescriptorHelpers.Load(objectInfo.Type, objectInfo.Name, objectInfo.Guid);

                //rootDescriptor ??= RootDescriptor.Create(objectInfo.Type, objectInfo.Name, objectInfo.Guid);

                rootDescriptor ??= Create();
                if (rootDescriptor is IValueDescriptor valueDescriptor)
                {
                    var value = valueDescriptor.Get();
                    var @return = new IOValue(objectInfo.Name, value);
                    return @return;
                }
                else
                {
                    throw new Exception("R 44 gfds");
                }
            }
            else if (operands.SingleOrDefault() is { Value: { } value } operand)
            {
                //DescriptorHelpers.Save(value, objectInfo.Name, objectInfo.Guid);

                rootDescriptor ??= Create();
                if (rootDescriptor is IValueDescriptor valueDescriptor)
                    valueDescriptor.Set(value);
                return null; ;
            }
            else
            {
                throw new Exception(" 445 44");
            }
        }

        private MemberDescriptor Create()
        {
            var instance = Activator.CreateInstance(objectInfo.Type);
            var rootDescriptor = new RootDescriptor(objectInfo.Type);
            var data = DescriptorConverter.ToDescriptor(instance, rootDescriptor);
            return data;
        }

        public ISerialise FromString(string str)
        {
            var objectInfo = JsonConvert.DeserializeObject<ObjectInfo>(str);
            return new ObjectOperation(objectInfo);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this.objectInfo);
        }



    }
}
