
using Bogus.DataSets;
using Splat;
using System;
using System.Diagnostics;
using Utility.Repos;
using Utility.Trees;

namespace Utility.Descriptors;

internal record ReferenceDescriptor(Descriptor Descriptor, object Instance) : ValuePropertyDescriptor(Descriptor, Instance), IReferenceDescriptor
{
    private readonly ITreeRepository repo = Locator.Current.GetService<ITreeRepository>();

    public override IObservable<object> Children
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
                    CompositeDisposable disp = new();
                    if(inst is null)
                    {
                        repo.SelectKeys(this.Guid)
                        .Subscribe(async a =>
                        {
                            foreach (var table in a)
                            {
                                if (table.Instance is { } item)
                                {
                                    DescriptorFactory.CreateRoot(item.GetType(), table.Guid, table.Name)
                                    .Subscribe(root =>
                                    {
                                        observer.OnNext(new(default, Changes.Type.Reset));
                                        observer.OnNext(new(root, Changes.Type.Add));
                                    });
                                }
                            }
                        });
                    }
                    if (inst is object obj)
                    {
                        var propertiesDescriptor = new PropertiesDescriptor(Descriptor, inst);
                        repo.Find(this.Guid, propertiesDescriptor.Name).Subscribe(pguid =>
                        {
                            observer.OnNext(new(propertiesDescriptor with { Guid = pguid }, Changes.Type.Add));
                        }).DisposeWith(disp);
                    }

                    if (inst is IEnumerable enumerable && inst is not string s && inst.GetType() is Type _type && _type.GetCollectionElementType() is Type elementType)
                    {
                        var collectionDescriptor = new CollectionDescriptor(Descriptor, elementType, enumerable);
                        repo
                        .Find(this.Guid, collectionDescriptor.Name)
                        .Subscribe(cguid =>
                        {
                            observer.OnNext(new(collectionDescriptor with { Guid = cguid }, Changes.Type.Add));
                        }).DisposeWith(disp);
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
}



