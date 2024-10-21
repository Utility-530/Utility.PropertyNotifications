
namespace Utility.Descriptors
{
    public record PropertiesDescriptor(Descriptor PropertyDescriptor, object Instance) : BasePropertyDescriptor(PropertyDescriptor, Instance), IPropertiesDescriptor
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
                        var propertyDescriptor = new PropertyDescriptor(descriptor, Instance) { Guid = Guid };
                        propertyDescriptor.Subscribe(changes);
                        observer.OnNext(new(propertyDescriptor, Changes.Type.Add));
                    }
                    return Disposable.Empty;
                });
            }
        }
    }
}