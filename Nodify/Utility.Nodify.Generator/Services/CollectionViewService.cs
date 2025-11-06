using System;
using System.Collections;
using System.Windows.Data;
using Utility.Models;
using Utility.PropertyNotifications;
using Utility.Services.Meta;

namespace Utility.Nodify.Generator.Services
{
    public record PredicateParam() : Param<CollectionViewService>(nameof(CollectionViewService.Set), "predicate", ParameterTypes: [typeof(Predicate<object>)]);
    public record ListInParam() : Param<CollectionViewService>(nameof(CollectionViewService.Create), "list");
    public record ListCollectionViewReturnParam() : Param<CollectionViewService>(nameof(CollectionViewService.Create));
    public class CollectionViewService : Model<ListCollectionView>
    {
        public ListCollectionView Create(IList list) => (ListCollectionView)(Value = new ListCollectionView(list));

        public void Set(Predicate<object> predicate)
        {
            this.WithChangesTo(a => a.Value).Subscribe(a =>
            {
                ((ListCollectionView)a).Filter = predicate;
            });
        }

    }
}
