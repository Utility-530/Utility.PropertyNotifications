using System.ComponentModel;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Nodes;
using Descriptor = System.ComponentModel.PropertyDescriptor;

namespace Utility.PropertyDescriptors
{
    public record MethodsDescriptor(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IMethodsDescriptor
    {
        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            var children = MethodExplorer.MethodInfos(Descriptor.PropertyType).ToArray();
            return Observable.Create<Change<IMemberDescriptor>>(observer =>
            {
                var descriptors = children.Select(methodInfo => new MethodDescriptor(methodInfo, Instance));
                foreach (var descriptor in descriptors)
                {
                    observer.OnNext(new(descriptor, Changes.Type.Add));
                }
                return Disposable.Empty;
            });
        }
    }
}


