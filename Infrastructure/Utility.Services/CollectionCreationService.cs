using System.Collections;
using System.Collections.ObjectModel;
using Utility.Changes;
using Utility.Interfaces.Generic.Data;
using Utility.Models;

namespace Utility.Services
{
    public record InstanceTypeParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Instance), "type");
    public record ListInstanceReturnParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Instance));
   
    public record ChangeParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Change), "change");
    public record ListParam() : MethodParameter<CollectionCreationService>(nameof(CollectionCreationService.Change), "list");



    public class CollectionCreationService
    {

        public static IList Instance(System.Type type)            
        {
            var instance = createCollectionInstance(type);
            subscribe(instance);
            return instance;

            static IList createCollectionInstance(System.Type type)
            {
                var constructedListType = typeof(ObservableCollection<>).MakeGenericType(type);
                var instance = (IList)Activator.CreateInstance(constructedListType);
                return instance;
            }

            static void subscribe(object? instance)
            {
                typeof(Utility.Persists.DatabaseHelper)
                    .GetMethod(nameof(Utility.Persists.DatabaseHelper.ToManager))
                    .MakeGenericMethod(instance.GetType())
                    .Invoke(null, parameters: [instance, new Func<object, Guid>(a => (a as IId<Guid>).Id), Utility.Constants.DefaultModelsFilePath]);
            }
        }

        public static void Change(Change change, IList list)
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
