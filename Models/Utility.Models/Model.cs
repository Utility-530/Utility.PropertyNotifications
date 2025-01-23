using Newtonsoft.Json;
using System;
using System.Collections;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Extensions;
using System.Linq;
using Utility.Changes;
using System.Collections.Generic;
using Utility.Helpers;
using Utility.Interfaces.NonGeneric;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Nodes.Filters;

namespace Utility.Models
{


    public class Model : NotifyPropertyClass, ISetNode, IProliferation, IClone
    {
        protected string m_name = "unknown";
        private INode node;
        int i = 0;
        private readonly Func<IEnumerable<Model>> func;
        protected SynchronizationContext? current;

        protected INodeSource source = Locator.Current.GetService<INodeSource>();


        public virtual Version Version { get; set; } = new();
        public required string Name
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
            current = SynchronizationContext.Current;
        }

        public Model(Func<IEnumerable<Model>> func) : this()
        {
            this.func = func;
        }

        [JsonIgnore]
        public INode Node
        {
            get => node; set
            {
                node = value;

                node.WithChangesTo(a => a.Parent)
                    .Where(a => a != default)
                    //.Take(1)
                    .Subscribe(a =>
                    Initialise(a));

                node.WithChangesTo(a => a.Key)
                .StartWith(node.Key)
                .Where(a => a != default)
                .Take(1)
                .Subscribe(a =>
                {
                    node.WithChangesTo(a => a.IsExpanded)
                    .StartWith(node.IsExpanded)
                    .Where(a => a)
                    .Take(1)
                    .Subscribe(a =>
                    {
                        InitialiseChildren(value);
                    });
                });

                node.WithChangesTo(a => a.Current)
                    .Where(a => a != null)
                    .Subscribe(a =>
                    {
                        Update(value, a);
                    });

                node.Items
                    .AndAdditions<IReadOnlyTree>()
                    .Subscribe(a =>
                    {
                        Addition(value, a);
                    });

                node.DescendantsAsync()
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
            }
        }

        public bool IsInitialising { get; set; }

        public virtual void SetNode(INode node)
        {
            Node = node;
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
                source.Remove(Node);
                return;
            }

            source
                .Find(Guid.Parse(parent.Key), Name, typeof(object), Node.LocalIndex)
                .Subscribe(guid =>
                {
                    Node.Key = new Keys.GuidKey(guid.Guid);
                    source.Add(Node);
                });
        }

        public virtual void InitialiseChildren(INode node)
        {
            IsInitialising = true;
            if (node.Key == "56dfda62-c8d8-4df6-ab05-faa9d728e841")
            {


            }

            source
                .ChildrenByGuidAsync(Guid.Parse(node.Key))
                .Subscribe(a =>
                {
                    if (a.Data?.ToString() == source.New)
                    {
                        CreateChildren()
                        .ForEach(child =>
                        {
                            node.Add(child);
                        });
                    }
                    else if (a.Data != null)
                    {
                        node.Add(a);
                    }
                    else
                    {

                    }
                },
                () =>
                {
                    IsInitialising = false;
                });
        }

        public virtual void AddDescendant(IReadOnlyTree node, int level)
        {
        }

        public virtual void SubtractDescendant(IReadOnlyTree node, int level)
        {
            source.Remove(Guid.Parse(node.Key));
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
            (node.Parent?.Data as Model)?.Update(node.Parent as IReadOnlyTree, current);
        }

        public virtual void Addition(IReadOnlyTree value, IReadOnlyTree a)
        {
        }

        public virtual void Subtraction(IReadOnlyTree value, IReadOnlyTree a)
        {
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
