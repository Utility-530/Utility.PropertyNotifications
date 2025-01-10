using Newtonsoft.Json;
using System;
using System.Collections;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Utility.Repos;
using Utility.PropertyNotifications;
using Utility.Reactives;
using Utility.Trees.Abstractions;
using Utility.Extensions;
using System.Linq;
using Utility.Changes;
using System.Collections.Generic;
using TreeView.Infrastructure;
using Utility.ViewModels;
using Utility.Interfaces.NonGeneric;
using Fasterflect;

namespace Utility.Nodes.Filters
{
    public class Model : NotifyPropertyChangedBase, ISetNode, IProliferation
    {
        protected string m_name = "unknown";
        private Node node;
        int i = 0;
        private readonly Func<IEnumerable<Node>> func;
        protected SynchronizationContext? current;

        public virtual Version Version { get; set; } = new();
        public string Name
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

        public Model(Func<IEnumerable<Node>> func) : this()
        {
            this.func = func;
        }

        [JsonIgnore]
        public Node Node
        {
            get => node; set
            {
                node = value;

                node.WithChangesTo(a => a.Parent)
                    .Where(a => a != default)
                    //.Take(1)
                    .Subscribe(a =>
                    Initialise((Node)a));

                node.WithChangesTo(a => a.Guid)
                .StartWith(node.Guid)
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
                        Update(value, (Node)a);
                    });

                node.Items
                    .AndAdditions<Node>()
                    .Subscribe(a =>
                    {
                        Addition(value, a);
                    });

                node.DescendantsAsync()
                    .Subscribe(a =>
                    {
                        if (a.Type == Changes.Type.Add)
                            this.AddDescendant(a.NewItem, a.Level);
                        else if (a.Type == Changes.Type.Remove)
                            this.SubtractDescendant(a.NewItem, a.Level);
                        else if (a.Type == Changes.Type.Update)
                            this.ReplaceDescendant(a.NewItem, a.OldItem, a.Level);
                        else
                            throw new Exception("Cds 333222");
                    });

                node.Items
                    .Subtractions<Node>()
                    .Subscribe(a =>
                    {
                        Subtraction(value, a);
                    });
            }
        }

        public bool IsInitialising { get; set; }

        public virtual void SetNode(Node node)
        {
            if (Node?.Equals(node) == true)
            {

            }
            this.Node = (Node)node;
        }

        public virtual void Initialise(Node parent)
        {
            //if (i++ > 1)
            //{
            //}

            if (parent == null)
            {
                NodeSource.Instance.Nodes.Remove(this.Node);
                return;
            }

            TreeRepository.Instance
                .Find((parent as IGuid).Guid, this.Name, typeof(object), this.Node.LocalIndex)
                .Subscribe(guid =>
                {
                    Node.Guid = guid.Guid;
                    NodeSource.Instance.Nodes.Add(this.Node);
                });

            var index = TreeRepository.Instance.MaxIndex(parent.Guid, Name + "_child") ?? 0;


            TreeRepository.Instance.Get(parent.Guid)
                .Subscribe(_d =>
                {
                    if (_d.Value == null)
                    {
                        IsInitialising = false;
                        return;
                    }
                    var value = _d.Value;
                    try
                    {
                        if (((NodeDTO)value).CurrentGuid != default)
                        {
                            var x = NodeSource.Instance
                            .SingleByGuidAsync(((NodeDTO)value).CurrentGuid)
                            .Subscribe(x =>
                            {
                                x.Parent = parent;
                                x.LocalIndex = index + 1;
                                //node.Current = x;
                                if (x.Data is ValueModel valueModel)
                                {
                                    //if (Value != null)
                                    //{
                                    //    valueModel.Value = Value;
                                    //    ValueModel_PropertyChanged(valueModel, new PropertyChangedEventArgs(nameof(Value)));
                                    //}
                                }
                                //Update(node, x);
                            });
                        }

                    }
                    catch (Exception ex)
                    {
                    }

                });
        }

        public virtual void InitialiseChildren(Node node)
        {
            IsInitialising = true;

            NodeSource.Instance
                .ChildrenByGuidAsync(node.Guid)
                .Subscribe(a =>
                {
                    if (a.Name == NodeSource.New)
                    {
                        ChildrenAsync()
                        .Subscribe(child =>
                        {
                            if (child.Type == Changes.Type.Add)
                            {
                                child.Value.Parent = node;
                                node.Add(child.Value);
                            }
                        });
                    }
                    else if (a.Data == null)
                    {
                        ChildrenAsync()
                        .Where(_a => _a.Value.Name == a.Name)
                        .Subscribe(child =>
                        {
                            if (child.Type == Changes.Type.Add)
                            {
                                child.Value.Parent = node;
                                node.Add(child.Value);
                            }
                        });
                    }
                    else if (node.Items.Cast<IGuid>().Select(a => a.Guid).Contains(a.Guid) == false)
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
                    //if (Version > new Version())
                    //{
                    //    ChildrenAsync()
                    //     .Subscribe(child =>
                    //     {
                    //         if (node.Items.Select(a => a.Name).Contains(child.Name))
                    //         {
                    //             return;
                    //         }
                    //         child.Parent = node;
                    //         node.Items.Add(child);
                    //     });
                    //} 
                    IsInitialising = false;
                });
        }

        public virtual void AddDescendant(IReadOnlyTree node, int level)
        {
        }

        public virtual void SubtractDescendant(IReadOnlyTree node, int level)
        {
        }

        public virtual void ReplaceDescendant(IReadOnlyTree @new, IReadOnlyTree old, int level)
        {
        }

        public virtual IObservable<Change<Node>> ChildrenAsync()
        {
            return Observable.Create<Change<Node>>(observer =>
            {
                foreach (var child in CreateChildren())
                    observer.OnNext(Change<Node>.Add(child));
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public virtual IEnumerable<Node> CreateChildren()
        {
            if (func != null)
                return func.Invoke();
            else
                return nodesFromProperties();

            IEnumerable<Node> nodesFromProperties()
            {
                foreach (var x in this.GetType().GetProperties().Select(a => (a.PropertyType, Attribute: a.Attribute<ChildAttribute>())).Where(a => a.Attribute != default))
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

                        yield return new Node(x.Attribute.Name, instance);
                    }
                }
            }
        }

        public virtual IEnumerable Proliferation()
        {
            yield break;
        }

        public virtual void Update(Node node, Node current)
        {
            (node.Parent?.Data as Model)?.Update(node.Parent as Node, current);
        }

        public virtual void Addition(Node value, Node a)
        {
        }

        public virtual void Subtraction(Node value, Node a)
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
