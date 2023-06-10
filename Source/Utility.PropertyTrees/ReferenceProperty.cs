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
            AddCommand = new ObservableCommand(async o =>
            {
                o.OnNext(false);

                if (Data is IList collection)
                {
                    var type = Data.GetType().GetGenericArguments().SingleOrDefault();
                    var instance = Activator.CreateInstance(type);
                    collection.Add(instance);
                    //await RefreshAsync();
                }

                o.OnNext(true);
            });
        }

        public ICommand AddCommand { get; }

        public override string Name => Descriptor?.Name ?? "Descriptor not set";

        public string DisplayName => Descriptor.DisplayName;

        public override bool IsReadOnly => Descriptor.IsReadOnly;

        public virtual string? Category => string.IsNullOrWhiteSpace(Descriptor.Category) ||
                Descriptor.Category.EqualsIgnoreCase(CategoryAttribute.Default.Category)
                    ? null
                    : Descriptor.Category;

        public virtual TypeConverter Converter => Descriptor.Converter;

        public override Type PropertyType => Descriptor.PropertyType;

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
                    disposable?.Dispose();
                    flag = false;
                    return await Task.FromResult(true);
                }
                flag = true;

                disposable = Observe<ChildrenResponse, ChildrenRequest>(new ChildrenRequest(Guid, Data))
                .Subscribe(a =>
                {
                    if (a.PropertyNode == null)
                    {
                        throw new Exception("dsv2s331hj f");
                    }
                    if (_children.Any(ass => a.PropertyNode.Key.Guid == (ass as ValueNode)?.Key.Guid) == false)
                        _children.Add(a.PropertyNode);

                    // if no more children arriving soon start adding from memory 
                    if (_children.Count == (Data as IEnumerable)?.Count())
                    {
                        if (typeof(INotifyCollectionChanged).IsAssignableFrom(PropertyType) == false)
                        {
                            _children.Complete();
                        }
                        RefreshOtherChildren(PropertyType.GenericTypes().SingleOrDefault());
                    }

                }, () =>
                {

                });
            }
            return await base.RefreshAsync();
        }

        private void RefreshOtherChildren(Type genericType)
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
                            _children.Add(CreateChild(genericType, a));
                        });
                    else
                    {

                    }
                }
            });

            PropertyBase CreateChild(Type genericType, ObjectCreationResponse a)
            {
                var propertyBase = a.Value as PropertyBase;
                var item = Activator.CreateInstance(genericType, Array.Empty<object>());
                propertyBase.Data = item;
                propertyBase.Descriptor = new CollectionItemDescriptor(item, _children.Count);
                return propertyBase;
            }
        }

        public override bool HasChildren => PropertyType != typeof(string);

        public override string ToString()
        {
            return Name;
        }

        public Type Switch(PropertyType propertyType)
        {
            return propertyType switch
            {
                Utility.PropertyTrees.Abstractions.PropertyType.Reference => typeof(ReferenceProperty),
                Utility.PropertyTrees.Abstractions.PropertyType.Value => typeof(ValueProperty),
                Utility.PropertyTrees.Abstractions.PropertyType.CollectionItem | Abstractions.PropertyType.Reference => typeof(CollectionItemReferenceProperty),
                Utility.PropertyTrees.Abstractions.PropertyType.CollectionItem | Abstractions.PropertyType.Value => typeof(CollectionItemValueProperty),
                Utility.PropertyTrees.Abstractions.PropertyType.Root => typeof(ReferenceProperty),
                _ => throw new Exception("f 33 dsf"),
            };
        }
    }
}