using System.Collections.ObjectModel;
using System.Reactive;
using ActivateAnything;
using Utility.Helpers.NonGeneric;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Meta;
using Utility.Reactives;

namespace Utility.PropertyDescriptors
{
    internal class CollectionDescriptor<T, TR>(string name) : CollectionDescriptor<T>(new RootDescriptor(typeof(T), typeof(TR), name), (IEnumerable)Activator.CreateInstance(typeof(T)))
    {
    }

    internal class CollectionDescriptor<T>(Descriptor PropertyDescriptor, IEnumerable Collection) : CollectionDescriptor(PropertyDescriptor, typeof(T).ElementType(), Collection), IGetType
    {
        public CollectionDescriptor(string name) : this(new RootDescriptor(typeof(T), null, name), new ObservableCollection<T>())
        {
        }
    }

    internal class CollectionDescriptor(Descriptor PropertyDescriptor, Type ElementType, IEnumerable collection) : MemberDescriptor(PropertyDescriptor, collection),
        ICollectionDescriptor,
        IRefresh,
        IGetType
    {
        private ObservableCollection<IDescriptor>? children;

        public static string _Name => "Collection";

        public override string? Name => _Name;

        public override IEnumerable Collection { get => collection; set => throw new Exception("FD£cwe"); }

        public override IEnumerable Items()
        {
            return children ??= build();

            ObservableCollection<IDescriptor> build()
            {
                var _collection = new ObservableCollection<IDescriptor>(new[]
                {
                    new CollectionHeadersDescriptor(ElementType,Descriptor.PropertyType) { Parent = this }
                });
                foreach (var x in addFromInstance())
                    _collection.Add(x);

                //this.Collection.Additions().Subscribe(item =>
                //{
                //    int i = (descriptors.LastOrDefault().Value.Index) + 1;
                //    children.Add(next(item, item.GetType(), Type, i));
                //});
                return _collection;
            }
        }

        public override int Count => Instance is IEnumerable enumerable ? enumerable.Count() : 0;

        public Type ElementType { get; } = ElementType;

        public override void Finalise(object? item = null)
        {
            var observer = Observer.Create<Change<IDescriptor>>((a) =>
            {
                if (a.Type == Utility.Changes.Type.Add && a.Value is IFinalise finalise)
                {
                    finalise.Finalise();
                }
            });
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RefreshEventArgs value)
        {
            //foreach (var item in addFromInstance())
            //    children.Add(item);
        }

        public void Refresh()
        {
            //foreach (var item in addFromInstance())
            //    children.Add(item);
        }

        public new Type GetType()
        {
            if (ParentType == null)
            {
                Type[] typeArguments = { Type };
                Type genericType = typeof(CollectionDescriptor<>).MakeGenericType(typeArguments);
                return genericType;
            }
            else
            {
                Type[] typeArguments = { Type, ParentType };
                Type genericType = typeof(CollectionDescriptor<,>).MakeGenericType(typeArguments);
                return genericType;
            }
        }

        public IEnumerable Proliferation()
        {
            var instance = Activator.CreateInstance(ElementType)!;
            int i = (Children.Cast<INodeViewModel>().LastOrDefault().Index.Local) + 1;
            yield return next(instance, ElementType, Type, i);
        }

        private IEnumerable<IDescriptor> addFromInstance()
        {
            int i = 0;
            foreach (var item in Collection)
            {

                i++;
                yield return next(item, item.GetType(), Type, i);
  
            }

            bool Contains(IEnumerable source, object value)
            {
                foreach (var i in source)
                {
                    if (Equals(i, value))
                        return true;
                }
                return false;
            }

        }
        IDescriptor next(object item, Type type, Type parentType, int i, bool refresh = false, DateTime? removed = null)
        {
            var descriptor = DescriptorConverter.ToDescriptor(new RootDescriptor(type, parentType, type.Name + $" [{i}]"), item);
            descriptor.Parent = this;
            if (refresh)
                descriptor.Initialise();
            return descriptor;
        }
    }
}