using Splat;
using Utility.Repos;

namespace Utility.Descriptors;

internal record ReferenceDescriptor(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance), IReferenceDescriptor
{
    private readonly ITreeRepository repo = Locator.Current.GetService<ITreeRepository>();
    private IObservable<Change<IDescriptor>> observable;

    public override IObservable<object> Children
    {
        get
        {
            if (Get() is var inst)
            {
                if (inst is Type type)
                {
                    return Observable.Empty<Change<IDescriptor>>();
                }
                return observable ??= Observable.Create<Change<IDescriptor>>(async observer =>
                {
                    CompositeDisposable disp = new();
                    if (inst is null)
                    {
                        int i = 0;

                        if (Descriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)) && Descriptor.PropertyType.IsAssignableTo(typeof(string)) == false && Descriptor.PropertyType.GetCollectionElementType() is Type _elementType)
                        {
                            repo
                            .Find(this.Guid, CollectionDescriptor._Name, Descriptor.PropertyType)
                            .Subscribe(cguid =>
                            {
                                var enumerable = (IEnumerable)Activator.CreateInstance(Descriptor.PropertyType);
                                var collectionDescriptor = new CollectionDescriptor(Descriptor, _elementType, enumerable);
                                if (i++ > 0)
                                {
                                    return;
                                }
                                collectionDescriptor.Subscribe(changes);
                                observer.OnNext(new(collectionDescriptor with { Guid = cguid }, Changes.Type.Add));
                            }).DisposeWith(disp);
                        }
                        else
                            repo.Find(this.Guid, "Properties", Descriptor.PropertyType)
                            .Subscribe(pguid =>
                            {
                                inst = Activator.CreateInstance(Descriptor.PropertyType);
                                var propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                                var propertiesDescriptor2 = propertiesDescriptor with { Guid = pguid };
                                propertiesDescriptor2.Subscribe(changes);
                                observer.OnNext(new(propertiesDescriptor2, Changes.Type.Add));
                            }).DisposeWith(disp);

                    }
                    if (inst is object obj)
                    {
                        observer.OnNext(new(default, Changes.Type.Reset));
                        var propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                        propertiesDescriptor.Subscribe(changes);

                        repo.Find(this.Guid, propertiesDescriptor.Name, Descriptor.PropertyType)
                        .Subscribe(pguid =>
                        {
                            observer.OnNext(new(propertiesDescriptor with { Guid = pguid }, Changes.Type.Add));
                        }).DisposeWith(disp);
                    }

                    if (inst is IEnumerable enumerable && inst is not string s && inst.GetType() is Type _type && _type.GetCollectionElementType() is Type elementType)
                    {
                        int i = 0;
                        var collectionDescriptor = new CollectionDescriptor(Descriptor, elementType, enumerable);
                        repo
                        .Find(this.Guid, collectionDescriptor.Name, Descriptor.PropertyType)
                        .Subscribe(cguid =>
                        {
                            if (i++ > 0)
                            {
                                return;
                            }
                            collectionDescriptor.Subscribe(changes);
                            observer.OnNext(new(collectionDescriptor with { Guid = cguid }, Changes.Type.Add));
                        }).DisposeWith(disp);
                    }

                    return Disposable.Empty;
                });
            }
            return Observable.Empty<Change<IDescriptor>>();
        }
    }
}



