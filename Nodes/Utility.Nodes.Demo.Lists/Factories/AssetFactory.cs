using Splat;
using System.Collections;
using System.Data.Entity.Core.Metadata.Edm;
using System.Reactive.Linq;
using Utility.Entities;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.Services;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public IObservable<INodeViewModel> BuildAssetRoot(Guid guid, Type type)
        {
            buildNetwork(guid);

            return nodeSource.Create(nameof(BuildAssetRoot),
                guid,
                s =>
                new Model(() => [
                    new Model<string>() { Name = search, DataTemplate = "SearchEditor" },
                    new ListModel(type) { Name = list, DataTemplate =  "SFGridTemplate"},
                    new EditModel(attach: node =>
                    {
                        //node
                        //.WithChangesTo(a => (a as IGetValue).Value)
                        //.Subscribe(model =>
                        //{
                        //    if (model is Asset creditCardModel)
                        //    {
                        //        creditCardModel
                        //        .WhenChanged()
                        //        .Subscribe(a =>
                        //        {
                        //            if(a.Name!= nameof(Asset.LastEdit))
                        //            {
                        //                creditCardModel.LastEdit = DateTime.Now;
                        //                creditCardModel.RaisePropertyChanged(nameof(Asset.LastEdit));
                        //            }
                        //        });
                        //    }
                        //});
                        node.ReactTo<SelectionReturnParam>(setAction: (a) => { node.Value = a; node.RaisePropertyChanged(nameof(EditModel.Value)); }, guid: guid);
                    })
                    { Name = edit },
                    new Model<string>() { Name = summary , DataTemplate = "MoneySumTemplate", IsValueTracked = false }
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
                { Name = main });

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