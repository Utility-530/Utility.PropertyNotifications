using System;
using System.Collections;
using System.Collections.ObjectModel;
using Utility.Models;

namespace Utility.Nodify.Generator.Services
{

    public record InstanceTypeParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Instance), "type");
    public record ListInstanceReturnParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Instance));

    public record ChangeParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Change), "change");
    public record ListParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Change), "list");



    public class CollectionCreationService
    {
        public static IList Instance(Type type)
        {
            var instance = createCollectionInstance(type);

            return instance;

            static IList createCollectionInstance(Type type)
            {
                var constructedListType = typeof(ObservableCollection<>).MakeGenericType(type);
                var instance = (IList)Activator.CreateInstance(constructedListType);

                return instance;
            }

        }

        public static void Change(Changes.Change change, IList list)
        {
            switch (change.Type)
            {
                case Changes.Type.Add:
                    list.Add(change.Value);
                    break;
                case Changes.Type.Remove:
                    list.Remove(change.Value);
                    break;
            }
        }
    }


}
