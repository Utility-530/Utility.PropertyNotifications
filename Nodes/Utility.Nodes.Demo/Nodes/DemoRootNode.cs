using AngleSharp;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Utility.Nodes.Demo.Infrastructure;
using Utility.Nodes.Reflections;
using Utility.Nodes.Solutions;
using Utility.Nodes.Types;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo
{
    public class DemoRootNode : Node<NodeType>
    {
        const string _guid = "c5eb6a09-2787-4b85-9f1c-bf9abb9ccb06";
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

        public override async Task<ITree> ToTree(object value)
        {
            if (value is NodeType nodeType)
                return nodeType switch
                {
                    NodeType.ViewModel => new RootViewModelNode(),
                    NodeType.Directory => new DirectoryNode(new System.IO.DirectoryInfo(@"C:\")),
                    NodeType.Assembly => new AssemblyNode(),
                    NodeType.Type => new TypeNode(),
                    NodeType.Object => create(),
                    NodeType.Html => createHtml(),
                    _ => new ExceptionNode(new Exception("Out of range"))
                    //_ => throw new Exception("r 4333"),
                };
            throw new Exception("2r 11 4333");

            DescriptorNode create()
            {
                var table = (LEDMessage)Activator.CreateInstance(typeof(LEDMessage));
                var root = DescriptorFactory.CreateRoot(table, "table_add");
                var reflectionNode = new DescriptorNode(root) { Parent = (ITree)this };
                return reflectionNode;
            }

            HtmlNode createHtml()
            {
                string htmlContent = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<title>Page Title</title>\r\n</head>\r\n<body>\r\n\r\n<h1>My First Heading</h1>\r\n<p>My first paragraph.</p>\r\n\r\n</body>\r\n</html>";
                var context = BrowsingContext.New();
                var document = context.OpenAsync(req => req.Content(htmlContent)).Result;
                var x = new HtmlNode(document.DocumentElement);
                return x;
            }
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

    public class ExceptionNode : Node<object>
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

        public override Task<ITree> ToTree(object value)
        {
            throw new NotImplementedException();
        }
    }
}

