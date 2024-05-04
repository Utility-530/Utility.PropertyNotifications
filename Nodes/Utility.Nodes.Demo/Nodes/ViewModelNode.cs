using System.Threading.Tasks;
using System;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Helpers;
using Utility.Nodes.Demo.Infrastructure;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using Utility.Trees.Abstractions;
using Utility.Keys;

namespace Utility.Nodes.Demo
{
    public class ViewModelNode : Node
    {
        private Type type;
        bool flag;

        public ViewModelNode(Type type)
        {
            this.type = type;
        }

        public override object Data => Activator.CreateInstance(type);

        public override string Key => new StringKey(type.AsString());

        public override System.IObservable<object?> GetChildren()
        {
            flag = true;
            return Observable.Create<object>(observer =>
            {
                foreach (var child in Resolver.Instance.Children(type))
                    observer.OnNext(child);
                return Disposable.Empty;
            });
      
        }

        public override string ToString()
        {
            return type.Name;
        }

        public override Task<IReadOnlyTree> ToNode(object value)
        {
            return Task.FromResult<IReadOnlyTree>(new ViewModelNode(value as Type));
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

