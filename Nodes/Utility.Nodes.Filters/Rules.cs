using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.Interfaces.Exs;
using Utility.Keys;
using Utility.Models.Trees;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using Utility.Helpers.Reflection;

namespace Utility.Nodes.Filters
{



    public class NodeInterface
    {

        private Lazy<Dictionary<string, PropertyInterface>> setdictionary = new(() =>
        {
            var dict  = typeof(Node)
                                               .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                               .Where(a => a.Name != nameof(IReadOnlyTree.Parent))
                                               .ToDictionary(a => a.Name, a => Rules.Decide(a));
            return dict;
        });

        public Getter Getter(string name) => setdictionary.Value.TryGetValue(name, out var @interface) ? @interface.Getter : null;
        public Setter Setter(string name) => setdictionary.Value.TryGetValue(name, out var @interface) ? @interface.Setter : null;
    }



    public class PropertyInterface
    {
        public string Name { get; set; }
        public Getter Getter { get; set; }
        public Setter Setter { get; set; }
    }


    public abstract class Setter
    {
        public virtual string Name { get; }

        public abstract void Set(object instance, object value);
    }
    
    public abstract class Getter
    {
        public virtual string Name { get; }

        public abstract object Get(object instance);
    }

    public class CurrentSetter : Setter
    {
        public override string Name => nameof(Node.Current);

        public override void Set(object instance, object value)
        {
            if(instance is INode node && Guid.TryParse(value.ToString(), out Guid guid))
                node.WithChangesTo(a => a.Key).Subscribe(a =>
            {
                Locator.Current.GetService<INodeSource>().FindChild(node, guid)
                                .Subscribe(a =>
                                {
                                    (instance as INode).Current = a;
                                });
            });
        }
    }

    public class CurrentGetter : Getter
    {
        public override string Name => nameof(Node.Current);

        public override object Get(object instance)
        {
            return (instance as INode).Current.Key;
        }
    }

    public class GenericSetter : Setter
    {
        private readonly Action<object, object> func;
        private readonly PropertyInfo prop;

        public GenericSetter(PropertyInfo prop)
        {
            Name = prop.Name;
            this.prop = prop;
        }

        public override string Name { get; }

        public override void Set(object instance, object value)
        {
            prop.SetValue(instance, value);
        }
    }

    
    public class GenericGetter : Getter
    {
        private readonly PropertyInfo prop;

        public GenericGetter(PropertyInfo prop)
        {
            this.prop = prop;
        }

        public override string Name { get => prop.Name; }

        public override object Get(object instance)
        {
            return prop.GetValue(instance);
        }
    }

    public static class Rules
    {
        public static PropertyInterface Decide(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == nameof(INode.Current))
            {
                return new PropertyInterface{ Name = propertyInfo.Name, Setter = new CurrentSetter(), Getter = new CurrentGetter() };
            }     
            if (propertyInfo.Name == nameof(ValueModel.Value))
            {
                return new PropertyInterface{ Name = propertyInfo.Name, Setter = new GenericSetter(propertyInfo), Getter = new GenericGetter(propertyInfo) };
            }
            else
            {
                return new PropertyInterface{ Name = propertyInfo.Name, Setter = new GenericSetter(propertyInfo), Getter = new GenericGetter(propertyInfo) };
            }
        }
    }

}
