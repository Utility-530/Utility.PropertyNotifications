using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility.Nodes.Abstractions;

namespace Utility.Nodes.Demo.Infrastructure
{
    public enum NodeType
    {
        ViewModel,
        Directory,
        //Property
    }


    public class TypeNode : Node
    {
        bool flag;
        public TypeNode()
        {
        }

        public override string Content => nameof(NodeType);

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

        protected override void SetChildrenCache(List<INode> childrenCache)
        {
            _branches.Clear();
            _branches.AddRange(childrenCache);
            _branches.Complete();
            base.SetChildrenCache(childrenCache);
        }
    }
}
