using Splat;
using Utility.Repos;

namespace Utility.Descriptors;

internal record ReferenceDescriptor(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance), IReferenceDescriptor
{
    private readonly ITreeRepository? repo = Locator.Current.GetService<ITreeRepository>();
    private IObservable<Change<IDescriptor>>? observable;
    private PropertiesDescriptor? propertiesDescriptor;

    public override IObservable<object> Children
    {
        get
        {
            return
                observable ??= Observable.Create<Change<IDescriptor>>(async observer =>
                {
                    CompositeDisposable disp = new();
                    this.WhenReceivedFrom(a => a.Value)
                    .Subscribe(inst =>
                    {
                        if (inst is null)
                        {
                            int i = 0;

                            if (Descriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)) && Descriptor.PropertyType.IsAssignableTo(typeof(string)) == false && Descriptor.PropertyType.GetCollectionElementType() is Type _elementType)
                            {
                                repo
                                .Find(this.Guid, CollectionDescriptor._Name, Descriptor.PropertyType)
                                .Subscribe(c =>
                                {
                                    var enumerable = (IEnumerable?)Activator.CreateInstance(Descriptor.PropertyType);
                                    var collectionDescriptor = new CollectionDescriptor(Descriptor, _elementType, enumerable);
                                    if (i++ > 0)
                                    {
                                        return;
                                    }
                                    collectionDescriptor.Subscribe(changes);
                                    observer.OnNext(new(collectionDescriptor with { Guid = c.Guid }, Changes.Type.Add));
                                }).DisposeWith(disp);
                            }
                            else
                                repo
                                .Find(this.Guid, "Properties", Descriptor.PropertyType)
                                .Subscribe(pguid =>
                                {
                                    if (inst != null)
                                        return;
                                    inst = Activator.CreateInstance(Descriptor.PropertyType);
                                    if (propertiesDescriptor != null)
                                    {
                                        observer.OnNext(new(propertiesDescriptor, Changes.Type.Remove));
                                    }
                                    propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst) { Guid = Guid };
                                    propertiesDescriptor.Subscribe(changes);
                                    observer.OnNext(new(propertiesDescriptor, Changes.Type.Add));
                                }).DisposeWith(disp);
                        }
                        if (inst is object obj)
                        {
                            observer.OnNext(new(default, Changes.Type.Reset));

                            repo
                            .Find(this.Guid, "Properties" /*propertiesDescriptor.Name*/, Descriptor.PropertyType)
                            .Subscribe(p =>
                            {
                                if (inst == null)
                                    return;
                                if (propertiesDescriptor != null)
                                {
                                    observer.OnNext(new(propertiesDescriptor, Changes.Type.Remove));
                                }
                                propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                                propertiesDescriptor.Subscribe(changes);

                                observer.OnNext(new(propertiesDescriptor with { Guid = p.Guid }, Changes.Type.Add));
                            }).DisposeWith(disp);
                        }
                        if (inst is IEnumerable enumerable && inst is not string s && inst.GetType() is Type _type && _type.GetCollectionElementType() is Type elementType)
                        {
                            int i = 0;
                            var collectionDescriptor = new CollectionDescriptor(Descriptor, elementType, enumerable);
                            repo
                            .Find(this.Guid, collectionDescriptor.Name, Descriptor.PropertyType)
                            .Subscribe(c =>
                            {
                                if (i++ > 0)
                                {
                                    return;
                                }
                                collectionDescriptor.Subscribe(changes);
                                observer.OnNext(new(collectionDescriptor with { Guid = c.Guid }, Changes.Type.Add));
                            }).DisposeWith(disp);
                        }
                    });
                    return disp;
                });
        }
    }
}



