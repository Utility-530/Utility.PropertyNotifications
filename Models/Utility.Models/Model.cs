using System.Collections;
using System.Reactive.Linq;
using Utility.Helpers;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions.Async;

namespace Utility.Models
{
    public class Model<T> : Model, IGet<T>, Interfaces.Generic.ISet<T>
    {
        private T? value = default;

        public Model()
        {
        }

        public Model(Func<IEnumerable<IReadOnlyTree>>? children = null, Action<IReadOnlyTree>? addition = null, Action<Model<T>>? attach = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : base(children, addition, attach: new Action<IReadOnlyTree>(a => attach?.Invoke((Model<T>)a)), raisePropertyCalled: raisePropertyCalled, raisePropertyReceived: raisePropertyReceived)
        {
        }

        public T? Get()
        {
            return this.value;
        }

        public void Set(T value)
        {
            this.value = value;
        }

        public override object Data { get; set; }

        public override object? Value
        {
            get { RaisePropertyCalled(value); return value; }
            set => this.RaisePropertyReceived<T>(ref this.value, (T)value);
        }

        public override object? Get(string name)
        {
            if (name == nameof(Value))
                return Get();
            return base.Get(name);
        }

        public override Type Type => typeof(T);

        public override void Set(object value, string name)
        {
            if (name == nameof(Value) && value is T tValue)
            {
                Set(tValue);
                return;
            }
            base.Set(value, name);
        }
    }

    public class Model : NodeViewModel, IClone, IYieldItems, IKey, IName, IAttach<IReadOnlyTree>, IProliferation
    {
        protected readonly Func<IEnumerable<IReadOnlyTree>>? childrenLambda;
        private readonly Action<IReadOnlyTree>? addition;
        private readonly Func<Model>? proliferation;
        private readonly Func<Type>? funcType;
        private readonly Action<IReadOnlyTree>? attach;

        public virtual Version Version { get; set; } = new();

        public Model()
        {
            Initialise();
        }

        public Model(Func<IEnumerable<IReadOnlyTree>>? childrenLambda = null, Action<IReadOnlyTree>? addition = null, Action<Model>? attach = null, Func<Model>? proliferation = null, Func<Type>? funcType = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : this()
        {
            this.childrenLambda = childrenLambda;
            this.isProliferable = childrenLambda == null;
            this.addition = addition;
            this.proliferation = proliferation;
            this.funcType = funcType;
            this.attach = new Action<IReadOnlyTree>(a => attach?.Invoke((Model)a));
        }

        public override Type Type => funcType?.Invoke() ?? base.Type;

        public virtual void Initialise()
        {
            this.WhenReceivedFrom(a => a.Current, includeNulls: true)
                .Skip(1)
                .Subscribe(a =>
                {
                    if (a != null)
                        if (this.Contains(a) == false)
                        {
                            if (this == a)
                            {
                                throw new Exception("SD 333333c");
                            }
                            this.Add(a);
                        }

                    Update(a);
                });

            this.Children
                .AndAdditions<IReadOnlyTree>()
                .Subscribe(a =>
                {
                    Addition(a);
                });
            this.Children
                .Subtractions<IReadOnlyTree>()
                .Subscribe(a =>
                {
                    Subtraction(a);
                });
            this.Children
                .Replacements<IReadOnlyTree>()
                .Subscribe(a =>
                {
                    Replacement(a.@new, a.old);
                });

            this.Descendants()
                .Subscribe(a =>
                {
                    if (a.Type == Utility.Changes.Type.Add)
                        AddDescendant(a.NewItem, a.Level);
                    else if (a.Type == Utility.Changes.Type.Remove)
                        SubtractDescendant(a.NewItem, a.Level);
                    else if (a.Type == Utility.Changes.Type.Update)
                        ReplaceDescendant(a.NewItem, a.OldItem, a.Level);
                    else
                        throw new Exception("Cds 333222");
                });
        }

        public virtual void AddDescendant(IReadOnlyTree node, int level)
        {
        }

        public virtual void SubtractDescendant(IReadOnlyTree node, int level)
        {
            if (node is IBreadCrumb)
                return;
            //var date = source.Remove(Guid.Parse(node.Key));
            //if (node is IRemoved removed)
            //{
            //    removed.Removed = date;
            //}
        }

        public virtual void ReplaceDescendant(IReadOnlyTree @new, IReadOnlyTree old, int level)
        {
        }

        public virtual IEnumerable Items()
        {
            if (childrenLambda != null)
                foreach (var child in childrenLambda())
                {
                    (child as ISetParent<IReadOnlyTree>).Parent = this;
                    yield return child;
                }
            else
                foreach (var child in nodesFromProperties())
                {
                    (child as ISetParent<IReadOnlyTree>).Parent = this;
                    yield return child;
                }

            IEnumerable<IReadOnlyTree> nodesFromProperties()
            {
                foreach (var x in GetType().GetProperties().Select(a => (a.PropertyType, Attribute: a.GetAttributeSafe<ChildAttribute>())).Where(a => a.Attribute.success))
                {
                    IReadOnlyTree instance = null;

                    if (x.PropertyType.IsAssignableTo(typeof(IReadOnlyTree)))
                    {
                        instance = (IReadOnlyTree)Activator.CreateInstance(x.PropertyType);
                    }
                    else if (x.Attribute.attribute.Type.IsAssignableTo(typeof(IReadOnlyTree)))
                    {
                        instance = (IReadOnlyTree)Activator.CreateInstance(x.Attribute.attribute.Type);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    (instance as ISetParent<IReadOnlyTree>).Parent = this;
                    (instance as ISetName).Name = x.Attribute.attribute.Name;

                    yield return instance;
                }
            }
        }

        public virtual void Update(IReadOnlyTree current)
        {
            this.WithChangesTo(a => (a as IGetParent<IReadOnlyTree>).Parent)
                .Subscribe(a =>
                {
                    (a as Model)?.Update(current);
                });
        }

        public virtual void Addition(IReadOnlyTree a)
        {
            if ((a as IGetParent<IReadOnlyTree>).Parent == null)
            {
                (a as ISetParent<IReadOnlyTree>).Parent = this;
                //if (a.Key != default)
                //    source.Add(a as INode);
                //else
                //{
                //}
            }

            if (a is IAttach<IReadOnlyTree> attach)
            {
                attach.Attach(a);
            }
            else if (a is IDescriptor)
            {
                //ignore
            }
            else
            {
                throw new Exception("Ds453 ddd");
            }
            addition?.Invoke(a);
        }

        public virtual void Replacement(IReadOnlyTree @new, IReadOnlyTree old)
        {
        }

        public virtual void Subtraction(IReadOnlyTree a)
        {
            //a.Parent = null;
        }

        public override sealed string ToString()
        {
            return Name;
        }

        public virtual object Clone()
        {
            var instance = ActivateAnything.Activate.New(this.GetType()) as Model;
            instance.Name = Name;
            return instance;
        }

        public virtual IEnumerable Proliferation()
        {
            if (proliferation is not null)
                yield return proliferation.Invoke();
            yield break;
        }

        public virtual void Attach(IReadOnlyTree value)
        {
            attach?.Invoke(value);
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ChildAttribute(string Name, System.Type Type = null) : Attribute
    {
        public string Name { get; } = Name;
        public System.Type? Type { get; } = Type;
    }
}