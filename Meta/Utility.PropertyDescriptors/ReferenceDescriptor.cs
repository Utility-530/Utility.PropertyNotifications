using Utility.Meta;

namespace Utility.PropertyDescriptors;

internal abstract record ReferenceDescriptor<T, TR>(string name) : ReferenceDescriptor(new RootDescriptor(typeof(T), typeof(TR), name), null)
{
}

internal record ReferenceDescriptor(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance), IReferenceDescriptor
{
    private PropertiesDescriptor? propertiesDescriptor;

    public override IEnumerable Children
    {
        get
        {

            if (Value is null)
            {
                int i = 0;

                if (Descriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)) && Descriptor.PropertyType.IsAssignableTo(typeof(string)) == false && Descriptor.PropertyType.GetCollectionElementType() is Type _elementType)
                {
                    var enumerable = (IEnumerable?)Activator.CreateInstance(Descriptor.PropertyType);
                    var collectionDescriptor = new CollectionDescriptor(Descriptor, _elementType, enumerable);
                    if (i++ > 0)
                    {
                        yield break;
                    }  
                    yield return collectionDescriptor;
                }
                else
                {
                    var inst = ActivateAnything.Activate.New(Descriptor.PropertyType);
                    propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst) { };
                    yield return propertiesDescriptor;
                }
            }
            if (Value is object obj)
            {
                yield return new PropertiesDescriptor(Descriptor, Value);
            }
            if (Value is IEnumerable _enumerable && Value is not string s && Value.GetType() is Type _type && _type.GetCollectionElementType() is Type elementType)
            {
                int i = 0;
                yield return new CollectionDescriptor(Descriptor, elementType, _enumerable);

            }

        }
    }
}



