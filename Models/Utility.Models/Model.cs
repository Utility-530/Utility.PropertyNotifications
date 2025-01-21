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


    public class Model : NotifyPropertyClass, ISetNode, IProliferation
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
            //if (Node?.Equals(node) == true)
            //{

            //}
            Node = node;
        }

        public virtual void Initialise(IReadOnlyTree parent)
        {
            //if (i++ > 1)
            //{
            //}

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

            var index = source.MaxIndex(Guid.Parse(parent.Key), Name + "_child") ?? 0;


            source.Get(Guid.Parse(parent.Key))
                .Subscribe(_d =>
                {
                    if (_d == null)
                    {
                        IsInitialising = false;
                        return;
                    }
                    var value = _d.Guid;
                    try
                    {
                        if (value != default)
                        {
                            var x = source
                            .SingleByGuidAsync(value)
                            .Subscribe(x =>
                            {
                                x.Parent = parent;
                                x.LocalIndex = index + 1;
                                //node.Current = x;
                                //if (x.Data is ValueModel valueModel)
                                //{
                                //    //if (Value != null)
                                //    //{
                                //    //    valueModel.Value = Value;
                                //    //    ValueModel_PropertyChanged(valueModel, new PropertyChangedEventArgs(nameof(Value)));
                                //    //}
                                //}
                                //Update(node, x);
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                });
        }

        public virtual void InitialiseChildren(INode node)
        {
            IsInitialising = true;

            source
                .ChildrenByGuidAsync(Guid.Parse(node.Key))
                .Subscribe(a =>
                {
                    if (a.Data?.ToString() == source.New)
                    {
                        ChildrenAsync()
                        .Subscribe(child =>
                        {
                            if (child.Type == Changes.Type.Add)
                            {
                                node.Add(child.Value);
                            }
                        });
                    }
                    else if (a.Data == null)
                    {
                        ChildrenAsync()
                        .Where(_a => _a.Value.Name == a.Data.ToString())
                        .Subscribe(child =>
                        {
                            if (child.Type == Changes.Type.Add)
                            {
                                node.Add(child.Value);
                            }
                        });
                    }
                    else if (node.Items.Cast<IKey>().Select(a => a.Key).Contains(a.Key) == false)
                    {
                        node.Add(a);
                        a.Parent = node;
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

        public virtual IObservable<Change<Model>> ChildrenAsync()
        {
            return Observable.Create<Change<Model>>(observer =>
            {
                foreach (var child in CreateChildren())
                    observer.OnNext(Change<Model>.Add(child));
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public virtual IEnumerable<Model> CreateChildren()
        {
            if (func != null)
                return func.Invoke();
            else
                return nodesFromProperties();

            IEnumerable<Model> nodesFromProperties()
            {
                foreach (var x in GetType().GetProperties().Select(a => (a.PropertyType, Attribute: a.GetAttribute<ChildAttribute>())).Where(a => a.Attribute != default))
                {
                    Model instance = null;
                    if (x.Attribute != null)
                    {
                        if (x.PropertyType.IsAssignableTo(typeof(Model)))
                        {
                            instance = (Model)Activator.CreateInstance(x.PropertyType);
                        }
                        else if (x.Attribute.Type.IsAssignableTo(typeof(Model)))
                        {
                            instance = (Model)Activator.CreateInstance(x.Attribute.Type);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                        instance.Name = x.Attribute.Name;

                        yield return instance;
                    }
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
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class ChildAttribute(string Name, System.Type Type = null) : Attribute
    {
        public string Name { get; } = Name;
        public System.Type? Type { get; } = Type;
    }
}
