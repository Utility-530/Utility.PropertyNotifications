using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Solutions;
using Utility.Nodes.Types;
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

        public override string Key => throw new NotImplementedException();

        public override IObservable<object> GetChildren()
        {
            flag = true;
             return Observable.Create<object>(observer =>
            {
                foreach (var x in Enum.GetValues(typeof(NodeType)))
                    observer.OnNext(x);
                return Disposable.Empty;
            });
        }

        public override async Task<IReadOnlyTree> ToNode(object value)
        {
            if (value is NodeType nodeType)
                return nodeType switch
                {
                    NodeType.ViewModel => new RootViewModelNode(),
                    NodeType.Directory => new DirectoryNode(new System.IO.DirectoryInfo(@"C:\")),             
                    NodeType.Assembly => new AssemblyNode(),
                    NodeType.Type => new TypeNode(),
                    _ => new ExceptionNode(new Exception("Out of range"))
                    //_ => throw new Exception("r 4333"),
                };
            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public override string ToString()
        {
            return Data;
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

        public override System.IObservable<object?> GetChildren()
        {
            return Observable.Return<object>(new object());
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(false);
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            throw new NotImplementedException();
        }
    }
}

