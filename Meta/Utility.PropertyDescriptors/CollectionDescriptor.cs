using System.Reactive;
using Utility.Helpers.NonGeneric;

namespace Utility.PropertyDescriptors
{
    public interface IValueCollection
    {
        IEnumerable Collection { get; }
    }

    internal record CollectionDescriptor(Descriptor PropertyDescriptor, Type ElementType, IEnumerable Collection) : BasePropertyDescriptor(PropertyDescriptor, Collection),
        ICollectionDescriptor,
        IValueCollection,
        IRefresh
    {
        List<ICollectionItemDescriptor> descriptors = new();

        public static string _Name => "Collection";

        public override string? Name => _Name;

        public override IEnumerable Children
        {
            get
            {
                yield return new CollectionHeadersDescriptor(ElementType, Instance.GetType());

                foreach (var x in AddFromInstance())
                {
                    yield return x;
                }
            }
        }



        public void Refresh()
        {
            //AddFromInstance(replaySubject);
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
                if (descriptors.Any(a => a.Item == item) == false)
                {
                    int i = (descriptors.LastOrDefault()?.Index ?? 0) + 1;
                    yield return Next(item, item.GetType(), Type, Changes.Type.Add, i);
                }
                else
                {

                }
            }

            foreach (var descriptor in descriptors.ToArray())
            {
                //i++;
                if (Contains(Collection, descriptor.Item) == false)
                {
                    yield return descriptor;
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
            var descriptor = DescriptorFactory.CreateItem(item, i, type, parentType) as ICollectionItemDescriptor;
            descriptors.Add(descriptor);
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
            throw new NotImplementedException();
        }
    }
}


