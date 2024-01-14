using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Nodes.WPF;
using Utility.Properties;
using Utility.Trees.Abstractions;
using Utility.WPF.Templates;

namespace Utility.Nodes.Demo
{


    public class DemoRootNode : Node
    {
        bool flag;
        public DemoRootNode()
        {
        }

        public override string Data => nameof(NodeType);

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
            return Data;
        }

        public override ITree ToNode(object value)
        {
            if (value is NodeType nodeType)
                return nodeType switch
                {
                    NodeType.ViewModel => new RootViewModelNode(),
                    NodeType.Directory => new DirectoryNode(@"C:\"),
                    NodeType.Model => new RootPropertyNode(),
                    NodeType.Assembly => new AssemblyNode(),
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

