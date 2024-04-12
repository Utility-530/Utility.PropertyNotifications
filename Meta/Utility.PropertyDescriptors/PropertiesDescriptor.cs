
namespace Utility.Descriptors
{
    internal record PropertiesDescriptor(Descriptor PropertyDescriptor, object Instance) : ReferenceDescriptor(PropertyDescriptor, Instance), IPropertiesDescriptor
    {
        public static string _Name => "Properties";
        public override string? Name => _Name;

        public override IObservable<object> Children
        {
            get
            {
                return Observable.Create<Change<IDescriptor>>(async observer =>
                {
                    var descriptors = TypeDescriptor.GetProperties(Instance);
                    foreach (Descriptor descriptor in descriptors)
                    {
                        var propertyDescriptor = await DescriptorFactory.ToValue(Instance, descriptor, Guid);
                        observer.OnNext(new(propertyDescriptor, Changes.Type.Add));
                    }
                    return Disposable.Empty;
                });
            }
        }

        public override object? Get()
        {

            return null;
        }

        public override void Set(object? value)
        {
            //Descriptor.SetValue(Instance, value);
        }

    }
}