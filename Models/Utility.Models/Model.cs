using Newtonsoft.Json;
using System.Collections;
using System.Reactive.Linq;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Trees.Extensions.Async;
using NetFabric.Hyperlinq;

namespace Utility.Models
{
    public class Model : NotifyPropertyClass, ISetNode, IProliferation, IClone, IYieldChildren, IKey, IName
    {
        protected string m_name = "unknown";
        private INode node;
        int i = 0;
        private readonly Func<IEnumerable<Model>> func;

        protected INodeSource source = Locator.Current.GetService<INodeSource>();
        protected Lazy<IContext> context = new(() => Locator.Current.GetService<IContext>());
        public virtual Version Version { get; set; } = new();
        public virtual required string Name
        {
            get => m_name;
            set
            {
                m_name = value;
                //base.RaisePropertyChanged();
            }
        }

        public Model()
        {
        }

        public Model(Func<IEnumerable<Model>> func) : this()
        {
            this.func = func;
        }

        [JsonIgnore]
        public INode Node
        {
            get => node;

            set
            {
                node = value;

                //node.WithChangesTo(a => a.Parent)
                //    .Where(a => a != default)
                //    //.Take(1)
                //    .Subscribe(a =>
                //    Initialise(a));

                //node.WithChangesTo(a => a.Key)
                //.StartWith(node.Key)
                //.Where(a => a != default)
                //.Take(1)
                //.Subscribe(a =>
                //{
                //    source.Add(Node);
                //});

                node.WithChangesTo(a => a.Current)
                    .Where(a => a != null)
                    .Subscribe(a =>
                    {
                        if (this.Node.Contains(a) == false)
                            this.Node.Add(a);

                        Update(value, a);
                    });

                node.Items
                    .AndAdditions<IReadOnlyTree>()
                    .Subscribe(a =>
                    {
                        Addition(value, a);
                    });

                node.Descendants()
                    .Subscribe(a =>
                    {
                        if (a.Type == Changes.Type.Add)
                            AddDescendant(a.NewItem, a.Level);
                        else if (a.Type == Changes.Type.Remove)
                            SubtractDescendant(a.NewItem, a.Level);
                        else if (a.Type == Changes.Type.Update)
                            ReplaceDescendant(a.NewItem, a.OldItem, a.Level);
                        else
                            throw new Exception("Cds 333222");
                    });

                node.Items
                    .Subtractions<IReadOnlyTree>()
                    .Subscribe(a =>
                    {
                        Subtraction(value, a);
                    });
                RaisePropertyChanged();
            }
        }

        public bool IsInitialising { get; set; }

        [JsonIgnore]
        public IEnumerable Children => CreateChildren();

        string IKey.Key { get => Node.Key; set => Node.Key = value; }

        public virtual void SetNode(INode node)
        {
            Node = node;
            node.IsPersistable = true;
        }

        bool isInitialised = false;
        public virtual void Initialise(IReadOnlyTree parent)
        {
            if (isInitialised)
            {

            }
            isInitialised = true;
            if (parent == null)
            {
                throw new Exception(" FSDsssdsss333");
                //source.Remove(Node);
                return;
            }
            //if (node.Key == default)
            //    source
            //        .Find(Guid.Parse(parent.Key), Name, type: this.GetType(), localIndex: Node.LocalIndex)
            //        .Subscribe(guid =>
            //        {
            //            Node.Key = new Keys.GuidKey(guid.Value.Guid);

            //        });
        }

        public virtual void AddDescendant(IReadOnlyTree node, int level)
        {
        }

        public virtual void SubtractDescendant(IReadOnlyTree node, int level)
        {
            if (node.Data is IBreadCrumb)
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

        public virtual IEnumerable<Model> CreateChildren()
        {
            if (func != null)
                return func.Invoke();
            else
                return nodesFromProperties();

            IEnumerable<Model> nodesFromProperties()
            {
                foreach (var x in GetType().GetProperties().Select(a => (a.PropertyType, Attribute: a.GetAttributeSafe<ChildAttribute>())).Where(a => a.Attribute.success))
                {
                    Model instance = null;

                    if (x.PropertyType.IsAssignableTo(typeof(Model)))
                    {
                        instance = (Model)Activator.CreateInstance(x.PropertyType);
                    }
                    else if (x.Attribute.attribute.Type.IsAssignableTo(typeof(Model)))
                    {
                        instance = (Model)Activator.CreateInstance(x.Attribute.attribute.Type);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    instance.Name = x.Attribute.attribute.Name;

                    yield return instance;

                }
            }
        }

        public virtual IEnumerable Proliferation()
        {
            yield break;
        }

        public virtual void Update(IReadOnlyTree node, IReadOnlyTree current)
        {

            if (node is INode _node)
            {

                _node.WithChangesTo(a => a.Parent)
                    .Subscribe(a =>
                    {
                        (a.Data as Model)?.Update(node.Parent, current);
                    });
            }
            //(node.Parent?.Data as Model)?.Update(node.Parent as IReadOnlyTree, current);
        }

        public virtual void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
            if (a.Parent == null)
            {
                a.Parent = value;
                //if (a.Key != default)
                //    source.Add(a as INode);
                //else
                //{

                //}
            }
        }

        public virtual void Subtraction(IReadOnlyTree value, IReadOnlyTree a)
        {
            //a.Parent = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public object Clone()
        {
            var instance = ActivateAnything.Activate.New(this.GetType()) as Model;
            instance.Name = Name;
            return instance;
        }


    }


    [AttributeUsage(AttributeTargets.Property)]
    public class ChildAttribute(string Name, System.Type Type = null) : Attribute
    {
        public string Name { get; } = Name;
        public System.Type? Type { get; } = Type;
    }
}
