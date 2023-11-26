using System.Threading.Tasks;
using System;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Helpers;

namespace Utility.Nodes.Demo.Infrastructure
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

        public override IEquatable Key => new StringKey(type.AsString());

        //public override IObservable Leaves => new Collection();

        public override async Task<object?> GetChildren()
        {
            flag = true;
            object? xx = await Task.Run(() => Resolver.Instance.Children(type));
            return xx;
        }

        public override string ToString()
        {
            return type.Name;
        }

        public override Node ToNode(object value)
        {
            return new ViewModelNode(value as Type);
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}

