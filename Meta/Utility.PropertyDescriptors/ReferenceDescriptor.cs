using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal class ReferenceDescriptor<T, TR>(string Name) : ReferenceDescriptor(new RootDescriptor(typeof(T), typeof(TR), Name), null)
{
}

internal class ReferenceDescriptor<T>(string Name) : ReferenceDescriptor(new RootDescriptor(typeof(T), null, Name), null)
{
}

internal class ReferenceDescriptor(Descriptor Descriptor, object Instance) : MemberDescriptor(Descriptor), IReferenceDescriptor, IGetType
{
    private PropertiesDescriptor? propertiesDescriptor;

    public override IEnumerable Items()
    {
        if (Instance is null)
        {
            int i = 0;

            if (Descriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)) && Descriptor.PropertyType.IsAssignableTo(typeof(string)) == false && this.CollectionItemPropertyType is Type _elementType)
            {
                var enumerable = (IEnumerable?)Activator.CreateInstance(Descriptor.PropertyType);
                var collectionDescriptor = new CollectionDescriptor(Descriptor, _elementType, enumerable) { Parent = this, Input = [], Output = [] };
                if (i++ > 0)
                {
                    yield break;
                }
                yield return collectionDescriptor;
            }
            else
            {
                var inst = ActivateAnything.Activate.New(Descriptor.PropertyType);
                propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst) { Parent = this, Input = [], Output = [] };
                yield return propertiesDescriptor;
            }
        }
        if (Instance is object obj)
        {
            yield return new PropertiesDescriptor(Descriptor, Instance) { Input = [], Output = [] };
        }
        if (Instance is IEnumerable _enumerable && Instance is not string s && Instance.GetType() is Type _type && _type.ElementType() is Type elementType)
        {
            int i = 0;
            yield return new CollectionDescriptor(Descriptor, elementType, _enumerable) { Parent = this, Input = [], Output = [] };

        }

    }

    public int Count => TypeDescriptor.GetProperties(Instance).Count;

    public object Instance { get; } = Instance;

    public new Type GetType()
    {
        if (ParentType == null)
        {
            Type[] typeArguments = { Type };
            Type genericType = typeof(ReferenceDescriptor<>).MakeGenericType(typeArguments);
            return genericType;
        }
        else
        {
            Type[] typeArguments = { Type, ParentType };
            Type genericType = typeof(ReferenceDescriptor<,>).MakeGenericType(typeArguments);
            return genericType;
        }
    }
}



