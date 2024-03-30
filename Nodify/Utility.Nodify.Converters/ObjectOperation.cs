using Newtonsoft.Json;
using Utility.Descriptors;
using Utility.Interfaces.NonGeneric;
using Utility.Nodify.Operations.Infrastructure;

namespace Utility.Nodify.Operations.Operations
{
    public class ObjectOperation : IOperation, ISerialise, IIsReadOnly, IValue, IType
    {
        private readonly ObjectInfo objectInfo;
        private PropertyDescriptor rootDescriptor;

        public ObjectOperation(ObjectInfo objectInfo)
        {
            this.objectInfo = objectInfo;
        }

        public object Value => objectInfo.Name;

        bool IIsReadOnly.IsReadOnly => true;

        Type IType.Type => typeof(string);

        public IOValue[] Execute(params IOValue[] operands)
        {
            if (operands.Any()==false)
            {
                //var instance = DescriptorHelpers.Load(objectInfo.Type, objectInfo.Name, objectInfo.Guid);

                //rootDescriptor ??= RootDescriptor.Create(objectInfo.Type, objectInfo.Name, objectInfo.Guid);

                rootDescriptor ??= Create();
                var value = rootDescriptor.Get();    
                var @return = new IOValue(objectInfo.Name, value);
                return new[] { @return };
            }
            else if(operands.SingleOrDefault() is { Value:{ } value } operand)
            {
                //DescriptorHelpers.Save(value, objectInfo.Name, objectInfo.Guid);

                 rootDescriptor ??= Create();

                rootDescriptor.Set(value);
                return Array.Empty<IOValue>();
            }
            else
            {
                throw new Exception(" 445 44");
            }
        }

        private PropertyDescriptor Create()
        {
            var instance = Activator.CreateInstance(objectInfo.Type);
            var rootDescriptor = new RootDescriptor(objectInfo.Type);
            var data = DescriptorFactory.ToValue(instance, rootDescriptor, objectInfo.Guid).GetAwaiter().GetResult();
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
