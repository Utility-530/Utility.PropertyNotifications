using Newtonsoft.Json;
using Splat;
using System;
using System.Collections;
using System.IO;
using System.Reactive.Linq;
using Utility.Entities.Comms;
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
using Utility.Services;
using Utility.ServiceLocation;
using Utility.Entities;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public IObservable<INodeViewModel> BuildEbayRoot(Guid guid, Type type)
        {
            buildNetwork(guid);

            return nodeSource.Create(nameof(BuildEbayRoot), guid, (s) =>
                new Model(() => [
                    new Model(()=>
                    [
                        new CommandModel<RefreshEvent>() { Name = refresh },
                        new StringModel(attach: searchModel=>{
                                  searchModel.Observe<FilterParam>(guid);
                        }) { Name = search,DataTemplate = "SearchEditor", Title = "Search"  },
                        new StringModel(attach: stringModel=>{
                               stringModel.Observe<BasePathParam>(guid);
                        } ) { Name = directory, DataTemplate = "DirectoryEditor",Title = "Base Directory" },
                        new StringModel( attach: stringModel =>{
                               stringModel.Observe<FilePathParam>(guid);
                        }) { Name = indexPath,
                        DataTemplate = "FilePathEditor",
                            Title = "Index Path"},
                    ],
                    attach : node=> {node.IsExpanded = true;  node.Orientation = Orientation.Horizontal; }                   ){ Name = controllerPath },
                    new ListModel(type, attach: listModel=>{

                        listModel.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a, guid : guid);

                        listModel.WhenReceivedFrom(a => a.Add, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Add))
                        .Observe<ChangeParam, Changes.Change>(guid);

                        listModel.WhenReceivedFrom(a => a.Remove, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Remove))
                        .Observe<ChangeParam, Changes.Change>(guid);

                        listModel.Observe<SelectionParam>(guid);

                    }) { Name = list1 },
                    new EditModel(attach: editModel =>
                    {
                        editModel.WithChangesTo(a => (a as IGetValue).Value)
                        .Subscribe(model =>
                        {
                            if (model is AuctionItem eModel)
                            {
                                //eModel.WhenChanged()
                                //.StartWith(default(PropertyChange))
                                //.Subscribe(a =>
                                //{
                                //    Current.GetService<RazorService>().OnNext(new Instance(model));
                                //});

                                eModel.WithChangesTo(a => a.RelativePath).Observe<FilePathParam, string>(guid: guid);

                            }
                        });
                        editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { (editModel as ISetValue).Value = a; editModel.RaisePropertyChanged(nameof(EditModel.Value)); }, guid: guid);

                    }) { Name = edit },
                    new Model<string>(attach: jsonModel => {

                        jsonModel.ReactTo<FullPathParam>(a =>
                        {
                            var path = Path.Combine(a.ToString(), "data.json");
                            if (!File.Exists(path))
                            {
                                jsonModel.Set( JsonConvert.SerializeObject(new { Error = "File does not exist: " + path }));
                            }
                            var text = File.ReadAllText(path);
                          jsonModel.Set(a.ToString());
                        }, guid);
                        }, raisePropertyCalled:false, raisePropertyReceived:false) {Name = details, DataTemplate = "Json"},
                    new StringModel() { Name = html, DataTemplate="Html" },
                    new ReadOnlyStringModel(attach: stringModel=>{
                              stringModel.ReactTo<RazorFileReturnParam>(setAction: a => stringModel.Set((string)a), guid: guid);
                    }) { Name = html1, DataTemplate = "HtmlEditor" },
                    new ReadOnlyStringModel(attach: rstringModel =>{
                         rstringModel.ReactTo<RazorFileReturnParam>(setAction: a => rstringModel.Set((string)a), guid: guid);
                    }) { Name = html2, DataTemplate = "HtmlWebViewer" },
                ],
                attach: (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; }
                )
                { Name = s });

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
