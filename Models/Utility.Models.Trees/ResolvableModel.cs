using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using DynamicData;
using Newtonsoft.Json;
using Utility.Collections;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Type = System.Type;

namespace Utility.Models.Trees
{
    public class ResolvableModel : BreadCrumbModel, IRoot
    {
        private CustomCollection<Type> types = new();
        private CustomCollection<PropertyInfo> properties = new();
        private CustomCollection<Assembly> assemblies = [];
        private GlobalAssembliesModel globalAssembliesModel;

        public ResolvableModel()
        {
            IsExpanded = true;
        }

        [JsonIgnore]
        public CustomCollection<Assembly> Assemblies
        {
            get => assemblies; set
            {
                assemblies = value;
            }
        }

        [JsonIgnore]
        public CustomCollection<Type> Types
        {
            get => types; set
            {
                types = value;
            }
        }

        [JsonIgnore]
        public CustomCollection<PropertyInfo> Properties
        {
            get => properties; set
            {
                properties = value;
            }
        }

        [JsonIgnore]
        public GlobalAssembliesModel GlobalAssembliesModel
        {
            get => globalAssembliesModel; set
            {
                globalAssembliesModel = value;
            }
        }

        public override IEnumerable<IReadOnlyTree> Items()
        {
            yield return globalAssembliesModel ??= new GlobalAssembliesModel { Name = "ass_root" };
        }

        public override void Addition(IReadOnlyTree add)
        {
            var _level = add.Level(this);

            switch (add)
            {
                case AssemblyTypePropertyModel { Assembly: { } _assembly, Type: { } _type, Value: PropertyInfo _property }:
                    {
                        assemblies.InsertSpecial(_level, _assembly);
                        types.InsertSpecial(_level, _type);
                        properties.InsertSpecial(_level, _property);
                        break;
                    }
                case AssemblyTypePropertyModel { Value: PropertyInfo _property } am:
                    {
                        am.WithChangesTo(a => a.Value).Subscribe(a =>
                        {
                            assemblies.InsertSpecial(_level, am.Assembly);
                            types.InsertSpecial(_level, am.Type);
                            properties.InsertSpecial(_level, _property);
                        });
                        break;
                    }
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        assemblies.InsertSpecial(_level, assembly);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        types.InsertSpecial(_level, type);
                        break;
                    }
                case PropertyModel { Value: PropertyInfo pInfo }:
                    {
                        properties.InsertSpecial(_level, pInfo);
                    }
                    break;

                case PropertyModel { } pm:
                    {
                        pm.WithChangesTo(a => a.Value).Cast<PropertyInfo>().Subscribe(x =>
                        {
                            properties.InsertSpecial(_level, x);
                        });
                    }
                    break;
            }
            base.Addition(add);
        }

        public override void Subtraction(IReadOnlyTree subtract)
        {
            switch (subtract)
            {
                case AssemblyTypePropertyModel { Assembly: { } _assembly, Type: { } _type, Value: { } _property }:
                    {
                        var level = subtract.Level(this);
                        assemblies.RemoveAtSpecial(level);
                        types.RemoveAtSpecial(level);
                        properties.RemoveAtSpecial(level);
                        break;
                    }
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        var level = subtract.Level(this);
                        assemblies.RemoveAtSpecial(level);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        var level = subtract.Level(this);
                        types.RemoveAtSpecial(level);
                        break;
                    }
                case PropertyModel { Value: { } pInfo }:
                    {
                        var level = subtract.Level(this);
                        properties.RemoveAtSpecial(level);
                    }
                    break;
            }
            base.Subtraction(subtract);
        }

        public override void Replacement(IReadOnlyTree @new, IReadOnlyTree old)
        {
            switch (@new)
            {
                case AssemblyModel { Assembly: { } assembly }:
                    {
                        var level = @new.Level(this);
                        assemblies.ReplaceSpecial(level, assembly);
                        break;
                    }
                case TypeModel { Type: { } type }:
                    {
                        var level = @new.Level(this);
                        types.ReplaceSpecial(level, type);
                        break;
                    }
                case PropertyModel { Value: PropertyInfo pInfo }:
                    {
                        var level = @new.Level(this);
                        properties.ReplaceSpecial(level, pInfo);
                        break;
                    }

                default:
                    throw new Exception("uiyi 333");
            }
            base.Replacement(@new, old);
        }

        public virtual bool TryGetValue(object instance, out object value)
        {
            value = instance;

            int i = 0;
            while (Properties.Count > i)
            {
                if (Assemblies[i] != value.GetType().Assembly)
                    return false;
                if (Types.Count > i == false)
                    return false;
                if (Properties.Count > i == false)
                    return false;
                if (value.GetType().Equals(Types[i]) == false)
                    return false;

                if (Properties[i].GetValue(value) is { } _value)
                {
                    i++;
                    value = _value;
                }
                else
                    return false;
            }
            return true;
        }

        public bool TrySetValue(object instance, object value)
        {
            if (Types.Any() == false)
                return true;
            int i = 0;
            while (Properties.Count > i + 1)
            {
                if (Assemblies[i] != instance.GetType().Assembly)
                    return false;
                if (Types.Count > i == false)
                    return false;
                if (Properties.Count > i == false)
                    return false;
                if (instance.GetType().Equals(Types[i]) == false)
                    return false;

                if (Properties[i].GetValue(instance) is { } _instance)
                {
                    i++;
                    instance = _instance;
                }
                else
                    return false;
            }
            Properties[i].SetValue(instance, value);
            return true;
        }
    }
}