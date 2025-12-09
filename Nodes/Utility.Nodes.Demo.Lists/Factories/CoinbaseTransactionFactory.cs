using System.Collections;
using System.Reactive;
using System.Reactive.Linq;
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
using Utility.ServiceLocation;
using Utility.Services;
using static Utility.API.Services.Coinbase;
using SFTemplates = Utility.Nodes.WPF.Templates.SyncFusion.Templates;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildCoinbaseTransactionRoot()
        {
            var guid = Guid.Parse(MetaDataFactory.coinbaseTransactionGuid);
            buildNetwork(guid);

            return 
                new Model(() => [
                    new CommandModel() { Name = "Import"},
                    new Model<string>() { Name = search, DataTemplate = Templates.SearchEditor},
                    new ListModel(MetaDataFactory.coinbaseTransactionType) { Name = list, DataTemplate =  SFTemplates.SFGridTemplate},
                    new Model<CryptoCoin>(attach: a =>{
                        a.Observe<CryptoCoinParam>(guid);
                    }){ Name = nameof(CryptoCoin), DataTemplate = Templates.EnumTemplate  },
                    new Model<Currency>(attach: a =>{
                        a.Observe<CurrencyParam>(guid);
                    }){Name = nameof(Currency), DataTemplate = Templates.EnumTemplate },
                    new Model<decimal>(attach: a =>{
                       a.ReactTo<ReturnPriceParam, decimal>(guid: guid);
                    }){Name = "SpotPrice",  DataTemplate = Templates.PriceTemplate, },

                    //new ListModel(type) { Name = list, DataTemplate =  "SFChartTemplate", XAxis = nameof(Transaction.Date), YAxis = nameof(Transaction.Balance00)},
                    //new StringModel() { Name = summary, DataTemplate = "MoneySumTemplate" }
                ],
                (addition) =>
                {
                    if (addition is Model<string> { Name: search } searchModel)
                    {
                        searchModel.Observe<FilterParam>(guid, includeInitial: true);
                    }
                    if (addition is CommandModel { } commandModel)
                    {
                        Observable.FromEvent(a => commandModel.Executed += a, a => commandModel.Executed -= a).Observe<InputParam, Unit>(guid);
                        commandModel.ReactTo<ReturnParam, Task>(setAction: async (a) => { commandModel.IsEnabled = false; await a; commandModel.IsEnabled = true; }, guid: guid);
                    }
                    if (addition is ListModel { } listModel)
                    {
                        listModel.ReactTo<ListCollectionViewReturnParam, IEnumerable>(setAction: (a) => listModel.Collection = a, guid: guid);
                        listModel.Observe<SelectionParam>(guid);
                    }
                    //if (addition is StringModel { Name: summary } summaryModel)
                    //{
                    //    summaryModel.ReactTo<SumAmountReturnParam>(guid: guid);
                    //}
                },
                attach: (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; })
                { Name = "coinbase",
                Guid = guid };

            static void buildNetwork(Guid guid)
            {
                var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
                serviceResolver.Connect<ListCollectionViewReturnParam, ListCollectionViewParam>();
                serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
            }
        }
    }
}