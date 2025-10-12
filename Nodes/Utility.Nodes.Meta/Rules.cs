#nullable enable
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models.Trees;
using Utility.Nodify.Operations.Infrastructure;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;

namespace Utility.Nodes.Meta
{
    public class NodeInterface
    {
        private Interfaces.Exs.INodeSource nodeSource;
        private Rules rules;
        private readonly Lazy<Dictionary<string, PropertyInterface>> setdictionary;

        public NodeInterface(Interfaces.Exs.INodeSource nodeSource)
        {
            this.nodeSource = nodeSource;
            this.rules = new Rules(nodeSource);
            this.setdictionary = new(() =>
            {
                var dict = typeof(NodeViewModel)
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Where(a => a.Name != nameof(IGetParent<>.Parent))
                                .ToDictionary(a => a.Name, a => rules.Decide(a));
                return dict;
            });
        }

        public Getter? Getter(string name) => setdictionary.Value.TryGetValue(name, out var @interface) ? @interface.Getter : null;
        public Setter? Setter(string name) => setdictionary.Value.TryGetValue(name, out var @interface) ? @interface.Setter : null;
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

        public abstract Task Set(object instance, object value);
    }

    public abstract class Getter
    {
        public virtual string Name { get; }

        public abstract object Get(object instance);
        public abstract bool Equality(object instance, object value);
    }

    public class CurrentSetter : Setter
    {
        private Interfaces.Exs.INodeSource nodeSource;

        public CurrentSetter(Interfaces.Exs.INodeSource nodeSource)
        {
            this.nodeSource = nodeSource;
        }

        public override string Name => nameof(NodeViewModel.Current);

        public override async Task Set(object instance, object value)
        {
            if (instance is INodeViewModel node)
            {
                Guid guid = default;
                if (value is INodeViewModel { Guid: Guid _guid })
                {
                    if (node.Cast<INodeViewModel>().SingleOrDefault(a => a == value) is { } current)
                    {
                        node.Current = current;
                        return;
                    }
                    guid = _guid;
                }
                else if (Guid.TryParse(value.ToString(), out Guid __guid))
                {
                    if (node.Cast<INodeViewModel>().SingleOrDefault(a => a.Guid == __guid) is { } current)
                    {
                        node.Current = current;
                        return;
                    }
                    guid = __guid;
                }
                else
                {
                    throw new Exception("ds 3££!!");
                }

                var task = await nodeSource.FindChild(node, guid).ToTask();
                node.Current = task;
            }
        }
    }

    public class CurrentGetter : Getter
    {
        public override string Name => nameof(NodeViewModel.Current);

        public override bool Equality(object instance, object value)
        {
            return value?.Equals(instance) ?? false;
            //if (instance is IGetKey { Key: { } key })
            //    return key == value;
            //if(instance == null)
            //    return false;
            //throw new Exception("ds 4444444332");
        }

        public override object Get(object instance)
        {
            return ((instance as IGet).Get(nameof(NodeViewModel.Current)) as IGetKey)?.Key;
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

        public override Task Set(object instance, object value)
        {
            //prop.SetValue(instance, value);
            (instance as ISet).Set(value, Name);
            if (instance is IRaiseChanges raiseChanges)
                raiseChanges.RaisePropertyReceived(value, null, Name);
            else
                throw new Exception("Dddazzzzz");
            return Task.CompletedTask;
        }
    }
    public class IgnoreSetter : Setter
    {
        public IgnoreSetter()
        {
        }

        public override Task Set(object instance, object value)
        {
            return Task.CompletedTask;

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

        public override bool Equality(object instance, object value)
        {
            return value?.Equals(instance) ?? false;
        }

        public override object Get(object instance)
        {
            return (instance as IGet).Get(Name);
        }
    }

    public class IgnoreGetter : Getter
    {


        public IgnoreGetter()
        {
        }
        
        public override bool Equality(object instance, object value)
        {
            return false;
        }

        public override object Get(object instance)
        {
            return null;
        }
    }

    public  class Rules
    {
        private Interfaces.Exs.INodeSource nodeSource;

        public Rules(Interfaces.Exs.INodeSource nodeSource)
        {
            this.nodeSource = nodeSource;
        }

        public  PropertyInterface Decide(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name == nameof(INodeViewModel.Current))
            {
                return new PropertyInterface { Name = propertyInfo.Name, Setter = new CurrentSetter(nodeSource), Getter = new CurrentGetter() };
            }
            if (propertyInfo.Name == nameof(ListModel.Add))
            {
                return new PropertyInterface { Name = propertyInfo.Name, Setter = new IgnoreSetter(), Getter = new IgnoreGetter() };
            }
            if (propertyInfo.Name == nameof(ListModel.Remove))
            {
                return new PropertyInterface { Name = propertyInfo.Name, Setter = new IgnoreSetter(), Getter = new IgnoreGetter() };
            }
            if (propertyInfo.Name == nameof(ValueModel.Value))
            {
                return new PropertyInterface { Name = propertyInfo.Name, Setter = new GenericSetter(propertyInfo), Getter = new GenericGetter(propertyInfo) };
            }
            else
            {
                return new PropertyInterface { Name = propertyInfo.Name, Setter = new GenericSetter(propertyInfo), Getter = new GenericGetter(propertyInfo) };
            }
        }
    }

}
