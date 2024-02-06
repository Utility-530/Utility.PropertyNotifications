using System.ComponentModel;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Utility.Changes;
using Utility.Nodes;
using Descriptor = System.ComponentModel.PropertyDescriptor;
using Utility.Nodes.Reflections;
using System.Reflection;

namespace Utility.PropertyDescriptors
{
    public record MethodsDescriptor(Descriptor Descriptor, object Instance) : PropertyDescriptor(Descriptor, Instance), IMethodsDescriptor
    {
        public static readonly string? _Name = "Methods";

        public override string? Name => _Name;

        public override IObservable<Change<IMemberDescriptor>> GetChildren()
        {
            var children = MethodExplorer.MethodInfos(Descriptor.PropertyType).ToArray();
            return Observable.Create<Change<IMemberDescriptor>>(async observer =>
            {
                var descriptors = children.Select(methodInfo => new MethodDescriptor(methodInfo, Instance));
                foreach (var descriptor in descriptors)
                {
                    var guid = await GuidRepository.Instance.Find(this.Guid, descriptor.Name);
                    descriptor.Guid = guid;
                    observer.OnNext(new(descriptor, Changes.Type.Add));
                }
                return Disposable.Empty;
            });
        }
    }
}


