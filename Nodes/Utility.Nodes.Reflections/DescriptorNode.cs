using Chronic;
using System.Reactive.Linq;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections
{
    public class DescriptorNode : NodeViewModel<IDescriptor>
    {
        IDescriptor data;

        public DescriptorNode(IDescriptor propertyData)
        {
            this.Data = propertyData;
            if (data is IYieldItems children)
                children.Items().Cast<IDescriptor>().ForEach(a => changes.OnNext(new(a, Utility.Changes.Type.Add)));
        }

        //public ReflectionNode()
        //{
        //}
        public override object Data
        {
            get => data;
            set
            {
                data = value as IDescriptor ?? throw new NullReferenceException($"{nameof(Data)} value is not of type {nameof(IDescriptor)}");
                Key = new TypeNameKey(data.Type, data.Name);
            }
        }

        //public override async Task<bool> HasMoreChildren()
        //{
        //    return /*data?.Type.IsValueOrString() == false && */await base.HasMoreChildren();
        //}

        public override string ToString()
        {
            return data?.Name ?? "No name";
        }


        public override Task<ITree> ToTree(object value)
        {
            if (value is IDescriptor { } descriptor)
            {
                return Task.FromResult((ITree)new DescriptorNode(descriptor) { Parent = this });
            }
            else
            {
                throw new Exception("32 2!pod");
            }
        }
    }
}
