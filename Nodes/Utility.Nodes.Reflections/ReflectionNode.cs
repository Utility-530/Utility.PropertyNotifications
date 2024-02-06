using System.Reactive.Linq;
using Utility.Collections;
using Utility.Models;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Reflections
{
    public class ReflectionNode : Node
    {
        IMemberDescriptor data;

        public ReflectionNode(IMemberDescriptor propertyData)
        {

            this.data = propertyData;
        }

        public ReflectionNode()
        {
        }

        public override object Data
        {
            get => data;
            set => data = value as IMemberDescriptor;
        }

        protected override async Task<bool> RefreshChildrenAsync()
        {
            if (await HasMoreChildren() == false)
                return true;
            //if (data.GetValue() is not { } inst)
            //    return true;

            items.Clear();
            data
               .GetChildren()
               .Subscribe(async a =>
               {
                   switch (a.Type)
                   {
                       case Changes.Type.Add:
                           {
                               if (a.Value is not { } desc)
                               {
                                   throw new Exception(" wrwe334 33");
                               }
                               else if (desc.Type == data.Type)
                               {
                                   var node = await ToNode(desc);
                                   items.Add(node);
                               }
                               else
                               {
                                   //var conversion = ObjectConverter.ToValue(inst, desc);
                                   var node = await ToNode(desc);
                                   items.Add(node);
                               }
                               break;
                           }
                       case Changes.Type.Remove:
                           {
                               items.RemoveOne(e => (e as IReadOnlyTree)?.Data.Equals(a.Value) == true);
                               break;
                           }
                       case Changes.Type.Reset:
                           {
                               items.Clear();
                               break;
                           }
                   }
               },
            e =>
            {
            },
            () =>
            {
                items.Complete();
            });

            return true;
        }


        public override IObservable<object?> GetChildren()
        {
            return data.GetChildren().Cast<object?>();
        }


        public override async Task<bool> HasMoreChildren()
        {
            return data.IsValueOrStringProperty == false && await base.HasMoreChildren();
        }

        public override string ToString()
        {
            return data?.Name;
        }


        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is IMemberDescriptor { Name: string name } descriptor)
            {

                return new ReflectionNode(descriptor) { Parent = this };
            }
            else
            {
                throw new Exception("32 2!pod");
            }
        }
    }
}
