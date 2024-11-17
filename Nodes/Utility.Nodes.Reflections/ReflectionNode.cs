using System.Reactive.Linq;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections
{
    public class ReflectionNode : Node<IDescriptor>
    {
        IDescriptor data;
    
        public ReflectionNode(IDescriptor propertyData)
        {
            this.data = propertyData;
            Key = new GuidKey(data.Guid);
            if (data is IChildren children)
                children.Children.Cast<Changes.Change<IDescriptor>>().Subscribe(changes);
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
                Key = new GuidKey(data.Guid);
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


        public override Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is IDescriptor { } descriptor)
            {
                return Task.FromResult((IReadOnlyTree)new ReflectionNode(descriptor) { Parent = this });
            }
            else
            {
                throw new Exception("32 2!pod");
            }
        }
    }
}
