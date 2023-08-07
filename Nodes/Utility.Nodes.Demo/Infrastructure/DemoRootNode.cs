using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Abstractions;
using Utility.Nodes.Demo.Infrastructure;

namespace Utility.Nodes.Demo
{
    public enum NodeType
    {
        ViewModel,
        Directory,
        //Property
    }


    public class DemoRootNode : Node
    {
        bool flag;
        public DemoRootNode()
        {
        }

        public override string Content => nameof(NodeType);

        public override IEquatable Key => throw new NotImplementedException();

        public override async Task<object?> GetChildren()
        {
            flag = true;
            return await Task.Run<object?>(() =>
            {

                return Enum.GetValues(typeof(NodeType));
            });
        }


        public override string ToString()
        {
            return Content;
        }

        public override INode ToNode(object value)
        {
            if (value is NodeType nodeType)
                return nodeType switch
                {
                    NodeType.ViewModel => new ViewModelNode(typeof(TopViewModel)),
                    NodeType.Directory => new DirectoryNode(@"C:\"),
                    //NodeType.Property => new RootProperty(Guid.NewGuid()) { Data = new Customer2() },
                    _ => throw new Exception("r 4333"),
                };
            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}
