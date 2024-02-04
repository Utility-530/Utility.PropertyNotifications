using Utility.Observables.Generic;
using Utility.Objects;
using Utility.Reactive.Helpers;
using Utility.Helpers;
using Utility.PropertyDescriptors;
using Utility.Models;
using Utility.Nodes.Reflections;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reactive.Disposables;

namespace Utility.Nodes
{
    public class RootPropertyNode : PropertyNode
    {
        public RootPropertyNode(object instance) : base(new PropertyData(new RootDescriptor(instance), instance))
        {

        }
    }

    public class PropertyNode : ReflectionNode
    {
        protected PropertyData data;

        public PropertyNode(PropertyData propertyData)
        {
            if (propertyData.Descriptor == null)
            {
            }
            this.data = propertyData;
        }

        public PropertyNode()
        {
        }


        public override object Data
        {
            get
            {
                return data;
            }
            set => data = value as PropertyData;
        }

        protected override async Task<bool> RefreshChildrenAsync()
        {
            if (await HasMoreChildren() == false)
                return true;
            if (data.Descriptor.GetValue(data.Instance) is not { } inst)
                return true;

            items.Clear();

            CompositeDisposable composite = new();

            if (ChildPropertyExplorer.ChildDescriptors(data.Descriptor).Any())
            {
                ToNode(new PropertiesData(data.Descriptor, data.Instance)).
                ToObservable()
                .Subscribe(node =>
                {
                    items.Add(node);
                }).DisposeWith(composite);
            }

            if (data.Descriptor.PropertyType.IsAssignableTo(typeof(Array)) == false)
                if (MethodExplorer.MethodInfos(data.Descriptor.PropertyType).Any())
                {
                    ToNode(new MethodsData(data.Descriptor, inst)).
                    ToObservable()
                    .Subscribe(node =>
                    {
                        items.Add(node);
                    }).DisposeWith(composite);
                }

            return true;

        }
        public override async Task<bool> HasMoreChildren()
        {
            return data.Descriptor.IsValueOrStringProperty() == false && await base.HasMoreChildren();
        }

        public override string ToString()
        {
            return data?.Descriptor.Name;
        }
    }
}

