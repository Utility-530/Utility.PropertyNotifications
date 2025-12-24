using System.Collections;
using System.Reactive.Linq;
using Splat;
using Utility.Entities;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
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
        public INodeViewModel BuildAssetRoot()
        {
            var guid = Guid.Parse(MetaDataFactory.assetGuid);
            buildNetwork(guid);

            return  new Model(() => [
                    new Model<string>() { Name = search, DataTemplate = Templates.SearchEditor },
                    new ListModel(MetaDataFactory.assetType) { Name = list, DataTemplate =  SFTemplates.SFGridTemplate},
                    new Model(attach: node =>
                    {
         
                        node.ReactTo<SelectionReturnParam>(setAction: (a) => { node.Value = a; node.RaisePropertyChanged(nameof(Model.Value)); }, guid: guid);
                    })
                    { 
                        Name = edit,
                        DataTemplate = Templates.EditTemplate,
                        ShouldValueBeTracked = false
                    },
                    new Model<string>() { Name = summary , DataTemplate = Templates.MoneySumTemplate,/* IsValueTracked = false*/ }
                ],
                (addition) =>
                {
                    if (addition is Model<string> { Name: search } searchModel)
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
                    if (addition is Model<string> { Name: summary } summaryModel)
                    {
                        summaryModel.ReactTo<ValueListReturnParam, decimal, string>(a => a.ToString("F2"), guid: guid);
                    }
                },
                (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; })
                { Guid = guid };

            static void buildNetwork(Guid guid)
            {
                var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
                serviceResolver.Connect<ListCollectionViewReturnParam, ListCollectionViewParam>();
                serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
                Locator.CurrentMutable.RegisterLazySingleton<AssetValueService>(() => new());
                Locator.CurrentMutable.RegisterLazySingleton<Utility.API.Services.GoldApi>(() => new());
                serviceResolver.Connect<ListInstanceReturnParam, ValueListInputParam>();
            }
        }
    }
}