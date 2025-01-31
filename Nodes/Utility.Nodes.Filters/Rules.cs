using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Filters
{
    public abstract class Setter
    {
        public virtual string Name { get; }

        public abstract void Set(object instance, object value);
    }

    public class CurrentSetter : Setter
    {
        public override string Name => nameof(Node.Current);

        public override void Set(object instance, object value)
        {
            (instance as Node).WithChangesTo(a => a.Key).Subscribe(a =>
            {
                NodeSource.Instance
                .FindNodeAsync(Guid.Parse(a), Guid.Parse(value.ToString()))
                .Subscribe(a =>
                {
                    (instance as INode).Current = a;
                });
            });
        }
    }

    public class GenericSetter : Setter
    {
        private readonly PropertyInfo prop;

        public GenericSetter(PropertyInfo prop)
        {
            this.prop = prop;
        }

        public override string Name { get => prop.Name; }

        public override void Set(object instance, object value)
        {
            prop.SetValue(instance, value);
        }
    }

    public static class Rules
    {
        public static Setter Decide(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == nameof(INode.Current))
            {
                return new CurrentSetter();
            }
            else
            {
                return new GenericSetter(propertyInfo);
            }
        }
    }

}
