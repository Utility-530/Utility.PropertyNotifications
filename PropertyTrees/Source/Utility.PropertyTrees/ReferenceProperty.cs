using DynamicData;
using System.Collections.Specialized;
using System.Windows.Input;
using Utility.Commands;
using Utility.Helpers;
using Utility.Helpers.NonGeneric;
using Utility.Infrastructure;
using Utility.Models;
using Utility.Nodes;
using Utility.Observables.Generic;
using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;

namespace Utility.PropertyTrees
{



    public class ReferenceProperty : PropertyBase
    {
        public ReferenceProperty(Guid guid) : base(guid)
        {
            //AddCommand = new ObservableCommand(async o =>
            //{
            //    o.OnNext(false);

            //    if (Data is IList collection)
            //    {
            //        var type = Data.GetType().GenericTypeArguments().SingleOrDefault();
            //        var instance = Activator.CreateInstance(type);
            //        collection.Add(instance);
            //        if (Data is not INotifyCollectionChanged collectionChanged)
            //        {
            //            RefreshAsync();
            //        }
            //        //await RefreshAsync();
            //    }

            //    o.OnNext(true);
            //});
        }

        //public ICommand AddCommand { get; }

        public override string Name => Descriptor?.Name ?? "Descriptor not set";

        public string DisplayName => Descriptor.DisplayName;

        public override bool IsReadOnly => Descriptor.IsReadOnly;

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public override Type PropertyType => Descriptor.PropertyType;

        // collection

        public virtual System.Type? CollectionItemPropertyType => PropertyType.IsArray ? PropertyType.GetElementType() : IsCollection ? PropertyType.GenericTypeArguments().SingleOrDefault() : null;
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
                if (flag == true)
                {
                    await Task.Delay(1000);
                    //disposable?.Dispose();
                    flag = false;
                    return await Task.FromResult(true);
                }
                flag = true;

                disposable = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Guid, Data))
                            .Subscribe(a =>
                            {
                                if (a.Include)
                                {
                                    if (a.PropertyNode == null)
                                    {
                                        throw new Exception("dsv2s331hj f");
                                    }

                                    if (_children.Any(ass => a.PropertyNode.Key.Guid == (ass as ValueNode)?.Key.Guid) == false)
                                    {
                                        a.PropertyNode.Parent = this;
                                        _children.Add(a.PropertyNode);
                                    }
                                }                  
                                // if no more children arriving soon start adding from memory 
                                if (_children.Count == (Data as IEnumerable)?.Count())
                                {
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
        }

        private void RefreshOtherChildren(Type componentType, Type genericType)
        {
            this
            .Observe<FindPropertyResponse, FindPropertyRequest>(new FindPropertyRequest(new Key(this.Guid, default, genericType)))
            .Subscribe(response =>
            {
                foreach (Key item in response.Value as IEnumerable)
                {
                    var switchType = Switch(Abstractions.PropertyType.CollectionItem | PropertyDescriptorHelper.GetPropertyType(item.Type));
                    if (_children.Any(ass => item.Guid == (ass as ValueNode)?.Key.Guid) == false)
                        Observe<ObjectCreationResponse, ObjectCreationRequest>(new(switchType, new[] { typeof(ValueNode), typeof(BaseObject) }, new object[] { item.Guid }))
                        .Subscribe(a =>
                        {
                            var child = CreateChild(a);
                            if (Data is IList collection)
                                collection.Add(child.Data);
                            child.Parent = this;    
                            _children.Add(child);
                        });
                    else
                    {

                    }
                }
            });

            PropertyBase CreateChild(ObjectCreationResponse a)
            {
                var propertyBase = a.Value as PropertyBase;
                var item = Activator.CreateInstance(genericType, Array.Empty<object>());
                propertyBase.Data = item;
                propertyBase.Descriptor = new CollectionItemDescriptor(item, _children.Count, componentType);
                return propertyBase;
            }
        }

        public override bool HasChildren => IsCollection || Data.GetType().GetProperties().Any();

        public override string ToString()
        {
            return Name;
        }

        public static Type Switch(PropertyType propertyType)
        {
            return propertyType switch
            {
                Abstractions.PropertyType.Reference => typeof(ReferenceProperty),
                Abstractions.PropertyType.Value => typeof(ValueProperty),
                Abstractions.PropertyType.CollectionItem | Abstractions.PropertyType.Reference => typeof(CollectionItemReferenceProperty),
                Abstractions.PropertyType.CollectionItem | Abstractions.PropertyType.Value => typeof(CollectionItemValueProperty),
                Abstractions.PropertyType.Root => typeof(ReferenceProperty),
                _ => throw new Exception("f 33 dsf"),
            };
        }
    }
}