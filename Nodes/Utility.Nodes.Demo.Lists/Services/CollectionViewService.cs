using System.Collections;
using System.Windows.Data;
using Utility.Models;

namespace Utility.Nodes.Demo.Lists.Services
{
    public record PredicateParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Set), "predicate");
    public record ListCollectionViewParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Set), "listCollectionView");
    public record ListInParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Create), "list");
    public record ListCollectionViewReturnParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Create));


    public class CollectionViewService
    {
        public ListCollectionView Create(IList list) => new(list);
        public void Set(Predicate<object> predicate, ListCollectionView listCollectionView)
        {
            listCollectionView.Filter = predicate;
        }
            
    }
}
