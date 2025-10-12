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
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.Services;
using Utility.ServiceLocation;

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
                        new StringModel(initialise: node=> {node.DataTemplate = "SearchEditor"; node.Title = "Search"; }, attach: searchModel=>{
                                  searchModel.Observe<FilterParam>(guid);
                        }) { Name = search },
                        new StringModel(initialise: node=> {node.DataTemplate = "DirectoryEditor"; node.Title = "Base Directory"; }, attach: stringModel=>{
                               stringModel.Observe<BasePathParam>(guid);
                        } ) { Name = directory },
                        new StringModel(initialise: node=> {node.DataTemplate = "FilePathEditor"; node.Title = "Index Path"; }, attach: stringModel =>{
                               stringModel.Observe<FilePathParam>(guid);
                        }) { Name = indexPath },
                    ],
                    node=> {node.IsExpanded = true;  node.Orientation = Orientation.Horizontal; },
                    (addition)=>{


                    }){ Name = controllerPath },
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
                            if (model is EbayModel eModel)
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
                    new Model<string>(initialise: n => n.DataTemplate = "Json", attach: jsonModel => {

                        jsonModel.ReactTo<FullPathParam>(a =>
                        {
                            var path = Path.Combine(a.ToString(), "data.json");
                            if (!File.Exists(path))
                            {
                                return JsonConvert.SerializeObject(new { Error = "File does not exist: " + path });
                            }
                            var text = File.ReadAllText(path);
                            return text;
                        }, a =>  jsonModel.Set(a.ToString()), guid: guid);
                        }, raisePropertyCalled:false, raisePropertyReceived:false) { Name = details },
                    new StringModel(initialise: n=> n.DataTemplate="Html") { Name = html },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlEditor", attach: stringModel=>{
                              stringModel.ReactTo<RazorFileReturnParam>(setAction: a => stringModel.Set((string)a), guid: guid);
                    }) { Name = html1 },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlWebViewer", attach: rstringModel =>{
                         rstringModel.ReactTo<RazorFileReturnParam>(setAction: a => rstringModel.Set((string)a), guid: guid);
                    }) { Name = html2 },
                ],
                (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; },
                (addition) =>
                {

                }
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
