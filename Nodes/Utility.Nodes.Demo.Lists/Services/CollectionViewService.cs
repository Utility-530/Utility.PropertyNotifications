using System.Collections;
using System.Windows.Data;
using Utility.Models;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{
    public record PredicateParam() :Param<CollectionViewService>(nameof(CollectionViewService.Set), "predicate");
    public record ListCollectionViewParam() :Param<CollectionViewService>(nameof(CollectionViewService.Set), "listCollectionView");
    public record ListInParam() :Param<CollectionViewService>(nameof(CollectionViewService.Create), "list");
    public record ListCollectionViewReturnParam() :Param<CollectionViewService>(nameof(CollectionViewService.Create));


    public class CollectionViewService
    {
        public ListCollectionView Create(IList list) => new(list);
        public void Set(Predicate<object> predicate, ListCollectionView listCollectionView)
        {
            listCollectionView.Filter = predicate;
        }
            
    }
}
