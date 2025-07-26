using System;
using System.Collections;
using System.Windows.Data;
using Utility.Models;

namespace Utility.Nodify.Generator.Services
{
    //public record ListCollectionViewParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Set), "listCollectionView");
    public record PredicateParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Set), "predicate");

    public record ListInParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Create), "list");
    public record ListCollectionViewReturnParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Create));


    public class CollectionViewService
    {
        ListCollectionView listCollectionView;
        public ListCollectionView Create(IList list) => listCollectionView = new(list);
        public void Set(Predicate<object> predicate)
        {
            listCollectionView.Filter = predicate;
        }
            
    }
}
