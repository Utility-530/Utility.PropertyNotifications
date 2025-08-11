using System;
using System.Collections;
using System.Windows.Data;
using Utility.Models;
using Utility.Models.Trees;
using Utility.PropertyNotifications;

namespace Utility.Nodify.Generator.Services
{
    //public record ListCollectionViewParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Set), "listCollectionView");
    public record PredicateParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Set), "predicate", ParameterTypes: [typeof(Predicate<object>)]);

    public record ListInParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Create), "list");
    public record ListCollectionViewReturnParam() : MethodParameter<CollectionViewService>(nameof(CollectionViewService.Create));


    public class CollectionViewService : ValueModel<ListCollectionView>
    {
        public ListCollectionView Create(IList list) => Value = new(list);

        public void Set(Predicate<object> predicate)
        {
            this.WithChangesTo(a => a.Value).Subscribe(a =>
            {
                a.Filter = predicate;
            });
        }

    }
}
