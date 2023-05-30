//using Utility.PropertyTrees.Abstractions;
//using System;
//using System.Threading.Tasks;

//namespace Utility.PropertyTrees.WPF.Demo.Infrastructure
//{
//    public class Engine : IPropertyGridEngine
//    {
//        private readonly PropertyNode propertyNode;


//        public Engine(PropertyNode propertyNode)
//        {
//            this.propertyNode = propertyNode;
//        }

//        public Task<IPropertyNode> Convert(object data)
//        {
//            propertyNode.Data = data;
//            return Task.FromResult((IPropertyNode)propertyNode);
//        }
//    }
//}