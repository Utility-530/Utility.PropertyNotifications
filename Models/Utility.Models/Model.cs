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
using Utility.Helpers.Reflection;
using Utility.Interfaces;

namespace Utility.Models
{
    public class ReadOnlyModel : Model
    {
        public ReadOnlyModel(Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null) : base(func, nodeAction, addition, false, false)
        {

        }
    }


    public class Model : Model<object>
    {
        public Model()
        {
        }

        public Model(Func<IEnumerable<IModel>> func, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : base(func, nodeAction, addition, raisePropertyCalled: raisePropertyCalled, raisePropertyReceived: raisePropertyReceived)
        {

        }
    }

    public class Model<T> : ValueModel<T>, ISetNode, IGetNode, IProliferation, IClone, IYieldChildren, IKey, IName 
    {
        private INode node;
        int i = 0;
        protected readonly Func<IEnumerable<IModel>>? func;
        private readonly Action<INode>? nodeAction;
        private readonly Action<IReadOnlyTree, IReadOnlyTree>? addition;
        protected INodeSource source = Locator.Current.GetService<INodeSource>();
        protected Lazy<IContext> context = new(() => Locator.Current.GetService<IContext>());
        public virtual Version Version { get; set; } = new();


        public Model(Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null, bool raisePropertyCalled = true, bool raisePropertyReceived = true) : base(raisePropertyCalled: raisePropertyCalled, raisePropertyReceived: raisePropertyReceived)
        {
            this.func = func;
            this.nodeAction = nodeAction;
            this.addition = addition;
        }

        [JsonIgnore]
        public INode Node
        {
            get => node;

            set
            {
                var previous = node;
                node = value;

                node.WithChangesTo(a => a.Current, includeNulls: true)
                    .Skip(1)
                    .Subscribe(a =>
                    {
                        if (a != null)
                            if (this.Node.Contains(a) == false)
                            {
                                if (this.Node == a)
                                {
                                    throw new Exception("SD 333333c");
                                }
                                this.Node.Add(a);
                            }

                        Update(value, a);
                    });

                node.Items
                    .AndAdditions<IReadOnlyTree>()
                    .Subscribe(a =>
                    {
                        Addition(value, a);
                    });

                node.Items
                    .Replacements<IReadOnlyTree>()
                    .Subscribe(a =>
                    {
                        Replacement(a.@new, a.old);
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
                RaisePropertyChanged(previous, value);
            }
        }

        //public bool IsInitialising { get; set; }

        [JsonIgnore]
        public IEnumerable Children => CreateChildren();

        string IKey.Key { get => Node.Key; set => Node.Key = value; }

        public virtual void SetNode(INode node)
        {
            Node = node;
            nodeAction?.Invoke(node);
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

        public virtual IEnumerable<IModel> CreateChildren()
        {
            if (func != null)
                return func.Invoke();
            else
                return nodesFromProperties();

            IEnumerable<IModel> nodesFromProperties()
            {
                foreach (var x in GetType().GetProperties().Select(a => (a.PropertyType, Attribute: a.GetAttributeSafe<ChildAttribute>())).Where(a => a.Attribute.success))
                {
                    IModel instance = null;

                    if (x.PropertyType.IsAssignableTo(typeof(Model)))
                    {
                        instance = (IModel)Activator.CreateInstance(x.PropertyType);
                    }
                    else if (x.Attribute.attribute.Type.IsAssignableTo(typeof(Model)))
                    {
                        instance = (IModel)Activator.CreateInstance(x.Attribute.attribute.Type);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                    (instance as ISetName).Name = x.Attribute.attribute.Name;

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
            addition?.Invoke(value, a);
        }

        public virtual void Replacement(IReadOnlyTree @new, IReadOnlyTree old)
        {

        }

        public virtual void Subtraction(IReadOnlyTree value, IReadOnlyTree a)
        {
            //a.Parent = null;
        }

        public sealed override string ToString()
        {
            return Name;
        }

        public virtual object Clone()
        {
            var instance = ActivateAnything.Activate.New(this.GetType()) as Model;
            instance.Name = Name;
            return instance;
        }


    }
    public class StringModel(Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null) : Model<string>(func, nodeAction, addition)
    {
        public StringModel() : this(null, null, null)
        {

        }
    }

    public class GuidModel(Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null) : Model<Guid>(func, nodeAction, addition)
    {
    }

    public class BooleanModel(Func<IEnumerable<IModel>>? func = null, Action<INode>? nodeAction = null, Action<IReadOnlyTree, IReadOnlyTree>? addition = null) : Model<bool>(func, nodeAction, addition)
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ChildAttribute(string Name, System.Type Type = null) : Attribute
    {
        public string Name { get; } = Name;
        public System.Type? Type { get; } = Type;
    }
}
