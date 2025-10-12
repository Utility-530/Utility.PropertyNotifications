using System.Collections;
using System.Reactive.Linq;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Models;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.Services;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public IObservable<INodeViewModel> BuildCreditCardRoot(Guid guid, Type type)
        {
            buildNetwork(guid);

            return nodeSource.Create(nameof(BuildCreditCardRoot),
                guid,
                s =>
                new Model(() => [
                    new StringModel(initialise: node=>node.DataTemplate = "SearchEditor") { Name = search },
                    new ListModel(type) { Name = list },
                    new EditModel(nodeAction: node =>
                    {
                        node
                        .WithChangesTo(a => (a as IGetValue).Value)
                        .Subscribe(model =>
                        {
                            if (model is CreditCardModel creditCardModel)
                            {
                                creditCardModel
                                .WhenChanged()
                                .Subscribe(a =>
                                {
                                    if(a.Name!= nameof(CreditCardModel.LastEdit))
                                    {
                                        creditCardModel.LastEdit = DateTime.Now;
                                        creditCardModel.RaisePropertyChanged(nameof(CreditCardModel.LastEdit));
                                    }
                                });
                            }
                        });
                    }) 
                    { Name = edit },
                    new StringModel(initialise: node=>node.DataTemplate = "MoneySumTemplate") { Name = summary }
                ],
                (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; },
                (addition) =>
                {
                    if (addition is EditModel { } editModel)
                    {
                        editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { editModel.Value = a; editModel.RaisePropertyChanged(nameof(EditModel.Value)); }, guid: guid);
                    }

                    if (addition is StringModel { Name: search } searchModel)
                    {
                        searchModel.Observe<FilterParam>(guid, includeInitial: true);
                    }

                    if (addition is ListModel { } listModel)
                    {
                        listModel.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a, guid: guid);

                        listModel.WhenReceivedFrom(a => a.Add, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Add))
                        .Observe<ChangeParam, Changes.Change>(guid);

                        listModel.WhenReceivedFrom(a => a.Remove, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Remove))
                        .Observe<ChangeParam, Changes.Change>(guid);

                        listModel.Observe<SelectionParam>(guid);
                    }
                    if (addition is StringModel { Name: summary } summaryModel)
                    {
                        summaryModel.ReactTo<SumCollectionReturnParam>(guid: guid);
                    }
                })
                { Name = main });

            static void buildNetwork(Guid guid)
            {
                var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
                serviceResolver.Connect<ListCollectionViewReturnParam, ListCollectionViewParam>();
                serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
                serviceResolver.Connect<ListInstanceReturnParam, SumCollectionInputParam>();
            }
        }
    }
}
