
using Splat;

namespace Utility.Descriptors;

public record ReferenceDescriptor(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IChildren
{
    private readonly ITreeRepository repo = Locator.Current.GetService<ITreeRepository>();

    public virtual IObservable<object> Children
    {
        get
        {
            if (Descriptor.GetValue(Instance) is var inst)
            {
                if (inst is Type type)
                {
                    return Observable.Empty<Change<IDescriptor>>();
                }
                return Observable.Create<Change<IDescriptor>>(async observer =>
                {
                    if (inst is object obj)
                    {
                        var propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                        var pguid = await repo.Find(this.Guid, propertiesDescriptor.Name);
                        observer.OnNext(new(propertiesDescriptor with { Guid = pguid }, Changes.Type.Add));
                    }

                    if (inst is IEnumerable enumerable && inst is not string s && inst.GetType() is Type _type && _type.GetCollectionElementType() is Type elementType)
                    {
                        var collectionDescriptor = new CollectionDescriptor(Descriptor, elementType, enumerable);
                        var cguid = await repo.Find(this.Guid, collectionDescriptor.Name);
                        observer.OnNext(new(collectionDescriptor with { Guid = cguid }, Changes.Type.Add));
                    }

                    //var methodsDescriptor = new MethodsDescriptor(Descriptor, inst);
                    //var mguid = await repo.Find(this.Guid, methodsDescriptor.Name);
                    //observer.OnNext(new(methodsDescriptor with { Guid = mguid }, Changes.Type.Add));
                    return Disposable.Empty;
                });
            }
            return Observable.Empty<Change<IDescriptor>>();
        }
    }

    public override void Initialise(object? item = null)
    {
        this.VisitChildren(a =>
        {
            if (a is PropertyDescriptor descriptor)
            {
                descriptor.Initialise();
            }
        });
    }

    public override void Finalise(object? item = null)
    {
        this.VisitChildren(a =>
        {
            if (a is PropertyDescriptor descriptor)
            {
                descriptor.Finalise();
            }
        });
    }

}



