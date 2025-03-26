using System.Collections.ObjectModel;
using System.Reactive;
using Utility.Helpers.NonGeneric;
using Utility.Meta;

namespace Utility.PropertyDescriptors
{
    public interface IValueCollection
    {
        IEnumerable Collection { get; }
    }

    internal record CollectionDescriptor<T, TR>(string name) : CollectionDescriptor<T>(new RootDescriptor(typeof(T), typeof(TR), name), (IEnumerable)Activator.CreateInstance(typeof(T)))
    {
    }

    internal record CollectionDescriptor<T>(Descriptor PropertyDescriptor, IEnumerable Collection) : CollectionDescriptor(PropertyDescriptor, typeof(T).ElementType(), Collection), IGetType
    {
        public CollectionDescriptor(string name) : this(new RootDescriptor(typeof(T), null, name), new ObservableCollection<T>())
        {
        }
    }

    internal record CollectionDescriptor(Descriptor PropertyDescriptor, Type ElementType, IEnumerable Collection) : BasePropertyDescriptor(PropertyDescriptor, Collection),
        ICollectionDescriptor,
        IValueCollection,
        IRefresh,
        IGetType
    {
        Dictionary<IDescriptor, (object Item, int Index)> descriptors = new();

        ObservableCollection<IDescriptor>? children;

        public static string _Name => "Collection";

        public override string? Name => _Name;

        public override IEnumerable Children
        {
            get
            {
                return children ??= new ObservableCollection<IDescriptor>(new[] { new CollectionHeadersDescriptor(ElementType, Instance.GetType()) }.Concat(AddFromInstance()));
            }
        }


        // collection
        public virtual int Count => Instance is IEnumerable enumerable ? enumerable.Count() : 0;

        public override void Finalise(object? item = null)
        {
            var observer = Observer.Create<Change<IDescriptor>>((a) =>
            {
                if (a.Type == Changes.Type.Add && a.Value is IFinalise finalise)
                {
                    finalise.Finalise();
                }
            });
        }

        IEnumerable<IDescriptor> AddFromInstance()
        {
            foreach (var item in Collection)
            {
                if (descriptors.Any(a => a.Value.Item == item) == false)
                {
                    int i = (descriptors.LastOrDefault().Value.Index) + 1;
                    yield return Next(item, item.GetType(), Type, Changes.Type.Add, i);
                }
                else
                {

                }
            }

            foreach (var descriptor in descriptors.ToArray())
            {
                //i++;
                if (Contains(Collection, descriptor.Value.Item) == false)
                {
                    yield return descriptor.Key;
                }
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

        IDescriptor Next(object item, Type type, Type parentType, Changes.Type changeType, int i, bool refresh = false, DateTime? removed = null)
        {
            var descriptor = DescriptorConverter.ToDescriptor(item, new RootDescriptor(type, parentType, type.Name + $" [{i}]"));
            descriptors.Add(descriptor, (item, i));
            if (refresh)
                descriptor.Initialise();
            return descriptor;
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
            foreach (var item in AddFromInstance())
                children.Add(item);
        }
        public void Refresh()
        {
            foreach (var item in AddFromInstance())
                children.Add(item);
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
    }
}


