using System;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Solutions;
using Utility.Trees.Abstractions;

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
                    NodeType.Assembly => new AssemblyNode(),
                    _ => new ExceptionNode(new Exception("Out of range"))
                    //_ => throw new Exception("r 4333"),
                };
            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }

    public class ExceptionNode : Node
    {
        private Exception exception;
        private object data;

        public ExceptionNode(Exception exception)
        {
            this.exception = exception;
        }

        public override object Data { get => exception; set => this.exception = (Exception)value; }

        public override Task<object?> GetChildren()
        {
            return Task.FromResult(new object());
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(false);
        }

        public override IReadOnlyTree ToNode(object value)
        {
            throw new NotImplementedException();
        }
    }
}

