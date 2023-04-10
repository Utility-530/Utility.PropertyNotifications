using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility.Collections;
using Utility.Nodes.Abstractions;
using Utility.Observables;

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

        public override object Content => Activator.CreateInstance(type);

        public override IObservable Leaves => new Collection();

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

        protected override void SetChildrenCache(List<INode> childrenCache)
        {
            _branches.Clear();
            _branches.AddRange(childrenCache);
            _branches.Complete();
            base.SetChildrenCache(childrenCache);
        }
    }
}
