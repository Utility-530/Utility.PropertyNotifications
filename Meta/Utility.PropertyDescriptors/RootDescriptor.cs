using Splat;
using System.Diagnostics;

namespace Utility.Descriptors
{
    public record RootDescriptor : PropertyDescriptor
    {
        public RootDescriptor(Descriptor Descriptor, object Instance) : base(Descriptor, Instance)
        {
        }

        public override object? GetValue()
        {
            if (Type.IsValueOrString())
            {
                if (Locator.Current.GetService<ITreeRepository>().Get(Guid) is { } x)
                    return x.Value;
                else
                {
                    var ree = (Type == typeof(string) ? null : Activator.CreateInstance(Type));
                    Locator.Current.GetService<ITreeRepository>().Set(Guid, ree, DateTime.Now);
                    return ree;
                }
            }
            else
            {
                var proto = Locator.Current.GetService<ITreeRepository>().CreateProto(Guid, Name, Type).Result;
                this.VisitDescendants(a => { });
                return base.GetValue();
            }
        }

        public override void SetValue(object value)
        {
            if (Type.IsValueOrString())
            {
                if (Locator.Current.GetService<ITreeRepository>().Get(Guid) is { } x && x.Value.Equals(value) == false)
                    Locator.Current.GetService<ITreeRepository>().Set(Guid, value, DateTime.Now);
            }
            else
            {
                var proto = Locator.Current.GetService<ITreeRepository>().CreateProto(Guid, Name, Type).Result;
                this.VisitDescendants(a => { });
            }
        }

        public static RootDescriptor Create(Type type, string name, Guid guid)
        {
            var instance = Activator.CreateInstance(type);
            var rootPropertyDescriptor = new RootPropertyDescriptor(type, name) { Item = instance };
            return new RootDescriptor(rootPropertyDescriptor, instance) { Guid = guid };
        }
    }


    public class RootPropertyDescriptor : Descriptor
    {
        public RootPropertyDescriptor(Type type, string? name = null) : base(type.Name ?? "root", null)
        {
            PropertyType = type;
            Name = name;
        }

        public override string Name { get; }
        public object Item { get; set; }

        public override Type ComponentType => null;

        public override bool IsReadOnly => true;

        public override Type PropertyType { get; }


        public override bool CanResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? component)
        {
            return Item ?? throw new Exception("sd sd");// Activator.CreateInstance(PropertyType);

        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? component, object? value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            throw new NotImplementedException();
        }
    }

    static class Helper
    {
        public static void VisitDescendants(this IMemberDescriptor tree, Action<IMemberDescriptor> action)
        {
            action(tree);

            tree.Children
                .Cast<Change<IMemberDescriptor>>()
                .Subscribe(a =>
                {
                    if (a.Type == Changes.Type.Add)
                    {
                        VisitDescendants(a.Value, action);
                        Trace.WriteLine(a.Value.ParentType + " " + a.Value.Type?.Name + " " + a.Value.Name);
                    }
                    else
                    {
                    }
                });
        }
    }
}