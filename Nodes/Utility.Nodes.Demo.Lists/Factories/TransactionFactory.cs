using System.Collections;
using System.Reactive.Linq;
using Utility.Entities;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Templates;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.Services;
using SFTemplates = Utility.Nodes.WPF.Templates.SyncFusion.Templates;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildTransactionRoot()
        {
            var guid = Guid.Parse(MetaDataFactory.transactionGuid);
            buildNetwork(guid);

            return 
                new Model(() => [
                    new Model<string>() { Name = search, DataTemplate = Templates.SearchEditor},
                    new ListModel(MetaDataFactory.transactionType) { Name = list, DataTemplate =  SFTemplates.SFGridTemplate},
                    new ListModel(MetaDataFactory.transactionType) { Name = list, DataTemplate =  SFTemplates.SFChartTemplate, XAxis = nameof(Transaction.Date), YAxis = nameof(Transaction.Balance00)},
                    new Model<string>() { Name = summary, DataTemplate = Templates.MoneySumTemplate }
                ],
                (addition) =>
                {
                    if (addition is Model<string> { Name: search } searchModel)
                    {
                        searchModel.Observe<FilterParam>(guid, includeInitial: true);
                    }

                    if (addition is ListModel { } listModel)
                    {
                        ServiceHelper.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a, guid: guid);

                        listModel.WhenReceivedFrom(a => a.Add, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Add))
                        .Observe<ChangeParam, Changes.Change>(guid);

                        listModel.WhenReceivedFrom(a => a.Remove, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Remove))
                        .Observe<ChangeParam, Changes.Change>(guid);

                        listModel.Observe<SelectionParam>(guid);
                    }
                    if (addition is Model<string> { Name: summary } summaryModel)
                    {
                        summaryModel.ReactTo<SumAmountReturnParam>(guid: guid);
                    }
                },
                attach: (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; })
                { Guid = guid };

            static void buildNetwork(Guid guid)
            {
                var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
                serviceResolver.Connect<ListCollectionViewReturnParam, ListCollectionViewParam>();
                serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
                serviceResolver.Connect<ListInstanceReturnParam, SumBalanceInputParam>();
            }
        }
    }
}