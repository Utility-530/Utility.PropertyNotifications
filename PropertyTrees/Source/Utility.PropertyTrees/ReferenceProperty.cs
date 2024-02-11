using System.Collections.Specialized;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;
using Utility.Nodes;
using Utility.Trees.Abstractions;
using Utility.Observables.Generic;
using Utility.PropertyDescriptors;


namespace Utility.PropertyTrees
{
    using CType = Changes.Type;

    public class ReferenceProperty : PropertyBase
    {
        public ReferenceProperty(Guid guid) : base(guid)
        {
        }

        public override string Name => Descriptor?.Name ?? "Descriptor not set";

        public string DisplayName => Descriptor.DisplayName;

        public override bool IsReadOnly => Descriptor.IsReadOnly;

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        //public virtual TypeConverter Converter => Descriptor.Converter;

        public override Type PropertyType => Descriptor.PropertyType;

        // collection

        public virtual Type? CollectionItemPropertyType => PropertyType.IsArray ? PropertyType.GetElementType() : IsCollection ? PropertyType.GenericTypeArguments().SingleOrDefault() : null;
        public virtual int CollectionCount => Value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;
        public virtual bool IsCollectionItemValueType => CollectionItemPropertyType != null && CollectionItemPropertyType.IsValueType;

        public override object Content => Name;

        public override object? Value
        {
            get
            {
                return Data;
            }
            set => throw new Exception("aa 4 43321``");
        }
        protected override async Task<bool> RefreshAsync()
        {
            if (IsCollection)
            {
                bool flag = false;
                disposable = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Guid, Data, Descriptor))
                            .Subscribe(response =>
                            {
                                if (response.NodeChange.Type == CType.Add)
                                {
                                    if (response.NodeChange.Value == null)
                                    {
                                        throw new Exception("dsv2s331hj f");
                                    }

                                    Add(response);
                                    return;
                                }
                                if (response.NodeChange.Type == CType.Remove)
                                {
                                    Remove(response);
                                    return;
                                }
                                if (response.NodeChange.Type == CType.Reset)
                                {
                                    Reset(response);
                                    return;
                                }
                                else
                                {

                                }
                                // if no more children arriving soon start adding from memory 
                                if (_children.Count == (Data as IEnumerable)?.Count())
                                {

                                    //if (flag == true)
                                    //{
                                    //}
                                    //flag = true;
                                    if (typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType) == false)
                                    {
                                        _children.Complete();
                                    }
                                    RefreshOtherChildren(PropertyType, CollectionItemPropertyType);
                                }

                            }, () =>
                            {

                            });
                return true;
            }

            return await base.RefreshAsync();

            void Add(ChildrenResponse response)
            {
                if (_children.Cast<IReadOnlyTree>().Any(ass => response.NodeChange.Value.Key.Equals(ass.Key)) == false)
                {
                    (response.NodeChange.Value as IReadOnlyTree).Parent = this;
                    _children.Add(response.NodeChange.Value);
                }
            }

            void Remove(ChildrenResponse response)
            {
                List<PropertyBase> removals = new();
                foreach (PropertyBase child in _children)
                {
                    if (child.Data == (response.SourceChange.Value as CollectionItemDescriptor).Item)
                        removals.Add(child);
                }
                if (removals.Count == 0)
                    throw new Exception("sd 67ssdsddddd");
                foreach (var remove in removals)
                {
                    _children.Remove(remove);
                    this.Observe<SetPropertyResponse, SetPropertyRequest>(new SetPropertyRequest(remove.Key, null))
                     .Subscribe(a =>
                     {
                         UpdateIndexesForChildren();
                     });

                    if (remove is not IReadOnlyTree { Key: Key key })
                    {
                        throw new Exception("sd w33!!£ £");
                    }
                }
            }

            void Reset(ChildrenResponse response)
            {
                List<object> removals = new();
                foreach (PropertyBase child in _children)
                {
                    removals.Add(child);
                }
                foreach (var remove in removals)
                {
            
                    if (remove is not IReadOnlyTree { Key: Key key })
                    {
                        throw new Exception("sd w33!!£ £");
                    }     
                    _children.Remove(remove);
                    this.Observe<SetPropertyResponse, SetPropertyRequest>(new SetPropertyRequest(key, null))
                        .Subscribe(a =>
                        {
    
                        });
                }


            }

            void RefreshOtherChildren(Type componentType, Type genericType)
            {
                this
                .Observe<FindPropertyResponse, FindPropertyRequest>(new FindPropertyRequest(new Key(this.Guid, default, genericType)))
                .Subscribe(response =>
                {
                    var responseChildren = response.Value as IEnumerable ?? throw new Exception("FVDDSF Sss");
                    foreach (Key key in responseChildren.Cast<Key>().OrderBy(a => a.Name))
                    {
                        if (_children.Cast<IReadOnlyTree>().Any(c => c.Key.Equals(key)))
                        {
                            continue;
                        }
                        var switchType = Switch(PropertyDescriptors.PropertyType.CollectionItem | PropertyDescriptorHelper.GetPropertyType(key.Type));
                        if (_children.Any(ass => key.Guid == (ass as ValueNode)?.Key.Guid) == false)
                            Observe<ObjectCreationResponse, ObjectCreationRequest>(new(switchType, new[] { typeof(ValueNode), typeof(BaseObject) }, new object[] { key.Guid }))
                            .Subscribe(a =>
                            {
                                int index = CollectionItemDescriptor.ToIndex(key.Name);
                                var child = CreateChild(a, key.Name, index);
                                if (Data is IList collection)
                                    collection.Insert(index, child.Data);
                                child.Parent = this;
                                _children.Add(child);
                            });
                        else
                        {
                        }
                    }
                });

                PropertyBase CreateChild(ObjectCreationResponse response, string name, int index)
                {
                    var propertyBase = response.Value as PropertyBase;
                    var item = Activator.CreateInstance(genericType, Array.Empty<object>());
                    propertyBase.Data = item;
                    propertyBase.Descriptor = new CollectionItemDescriptor(item, index, componentType);
                    return propertyBase;
                }
            }

            void UpdateIndexesForChildren()
            {
                int i = 0;
                foreach (PropertyBase child in _children)
                {
                    var key = child.Key;
                    child.Descriptor = new CollectionItemDescriptor(child.Data, i++, PropertyType);
                    this
                        .Observe<SetPropertyResponse, SetPropertyRequest>(new SetPropertyRequest(key, child.Key))
                        .Subscribe(response =>
                        {
                        });
                }
            }
        }


        public override bool HasChildren => IsCollection || Descriptor.GetChildProperties().Count > 0 || PropertyType.GetMethods().Any();

        public override string ToString()
        {
            return Name;
        }

        public static Type Switch(PropertyType propertyType)
        {
            return propertyType switch
            {
                PropertyDescriptors.PropertyType.Reference => typeof(ReferenceProperty),
                PropertyDescriptors.PropertyType.Value => typeof(ValueProperty),
                PropertyDescriptors.PropertyType.CollectionItem | PropertyDescriptors.PropertyType.Reference => typeof(CollectionItemReferenceProperty),
                PropertyDescriptors.PropertyType.CollectionItem | PropertyDescriptors.PropertyType.Value => typeof(CollectionItemValueProperty),
                PropertyDescriptors.PropertyType.Root => typeof(ReferenceProperty),
                _ => throw new Exception("f 33 dsf"),
            };
        }

       
    }
}