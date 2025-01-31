
using Splat;
using Utility.Repos;

namespace Utility.Descriptors
{
    public record PropertiesDescriptor(Descriptor PropertyDescriptor, object Instance) : BasePropertyDescriptor(PropertyDescriptor, Instance), IPropertiesDescriptor
    {
        private readonly ITreeRepository repo = Locator.Current.GetService<ITreeRepository>();

        public static string _Name => "Properties";
        public override string? Name => _Name;

        public override IObservable<object> Children
        {
            get
            {
                return Observable.Create<Change<IDescriptor>>(async observer =>
                {
                    var descriptors = TypeDescriptor.GetProperties(Instance);
                    CompositeDisposable composite = [];
                    foreach (Descriptor descriptor in descriptors)
                    {
                        repo.Find(Guid, descriptor.Name, type: descriptor.PropertyType)
                        .Subscribe(a =>
                        {
                            var propertyDescriptor = new PropertyDescriptor(descriptor, Instance) { Guid = a.Value.Guid };
                            propertyDescriptor.Subscribe(changes);
                            observer.OnNext(new(propertyDescriptor, Changes.Type.Add));
                        }).DisposeWith(composite);
                    }
                    observer.OnCompleted();
                    return composite;
                });
            }
        }
    }
}