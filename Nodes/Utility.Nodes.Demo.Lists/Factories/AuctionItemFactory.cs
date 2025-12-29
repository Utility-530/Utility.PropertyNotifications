using System;
using System.Collections;
using System.IO;
using System.Reactive.Linq;
using Newtonsoft.Json;
using Splat;
using UnitsNet;
using Utility.Entities;
using Utility.Entities.Comms;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.NonGeneric;
using Utility.Keys;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyDescriptors;
using Utility.PropertyNotifications;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Trees;
using Utility.Interfaces;
using Utility.Trees.Extensions.Async;
using Utility.Models.Templates;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildEbayRoot()
        {
            var guid = Guid.Parse(MetaDataFactory.auctionItemGuid);
            buildNetwork(guid);

            return
                new Model(() => [
                    new Model(()=>
                    [
                        new CommandModel<RefreshEvent>() { Name = refresh },
                        new Model<string>(attach: searchModel=> {
                                  searchModel.Observe<FilterParam>(guid);
                        })
                        {
                            Name = search,
                            DataTemplate = Templates.SearchEditor,
                            Title = "Search"
                        },
                        new Model<string>(attach: stringModel=> {
                               stringModel.Observe<BasePathParam>(guid);
                        } )
                        {
                            Name = directory,
                            DataTemplate = Templates.DirectoryEditor,
                            Title = "Base Directory"
                        },
                        new Model<string>( attach: stringModel => {
                        stringModel.Observe<FilePathParam>(guid);
                        })
                        {
                            Name = indexPath,
                            DataTemplate = Templates.FilePathEditor,
                            Title = "Index Path"
                        },
                    ],
                    attach : node=> {
                        node.IsExpanded = true;
                        node.Orientation = Orientation.Horizontal;
                    }){
                        Name = controllerPath
                    },
                    new Model(attach: listModel =>
                    {
                        ServiceHelper.ReactTo<ListCollectionViewReturnParam, IEnumerable>(a => listModel.Collection = a, guid);
                        listModel.Observe<SelectionParam>(guid);
                    }) {
                        Name = list1,
                        DataTemplate = Templates.DataGridTemplate,
                        ShouldValueBeTracked = false,
                        Type = MetaDataFactory.auctionItemType
                    },

                    new Model(attach: editModel =>
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
                        ServiceHelper.ReactTo<OutValueParam>(setAction: (a) => { (editModel.Children as IList).Clear(); editModel.Add(a); }, guid: guid);
                    }, funcType: ()=>  typeof(AuctionItem)) {
                        Name = edit,
                        ShouldValueBeTracked = false,
                        //DataTemplate = "EditTreeTemplate",
                    },
                    //new Model<string>(attach: jsonModel => {
                    //    jsonModel.ReactTo<FullPathParam>(a =>
                    //    {
                    //        var path = Path.Combine(a.ToString(), "data.json");
                    //        if (!File.Exists(path))
                    //        {
                    //            jsonModel.Set( JsonConvert.SerializeObject(new { Error = "File does not exist: " + path }));
                    //        }
                    //        var text = File.ReadAllText(path);
                    //      jsonModel.Set(a.ToString());
                    //    }, guid);
                    //    }, raisePropertyCalled:false, raisePropertyReceived:false) {Name = details, DataTemplate = "Json"},
                    new Model<string>(attach: a=>{
                        ServiceHelper.ReactTo<GeneralOutStringParam>(setAction: _a =>
                        {
                            a.Set((string)_a);
                            a.RaisePropertyChanged("Value");
                        }, guid: guid);
                    }) { Name = html, DataTemplate=Templates.Html },
                    new Model<string>(attach: stringModel=>{
                              //stringModel.ReactTo<RazorFileReturnParam>(setAction: a => stringModel.Set((string)a), guid: guid);
                              ServiceHelper.ReactTo<GeneralOutStringParam>(setAction: _a =>    {
                            stringModel.Set((string)_a);
                            stringModel.RaisePropertyChanged("Value");
                        }, guid: guid);
                    }) { Name = html1, DataTemplate = Templates.HtmlEditor },
                    new Model<string>(attach: rstringModel =>{
                         //rstringModel.ReactTo<RazorFileReturnParam>(setAction: a => rstringModel.Set((string)a), guid: guid);
                         ServiceHelper.ReactTo<GeneralOutStringParam>(setAction: _a => {
                             rstringModel.Set((string)_a);
                             rstringModel.RaisePropertyChanged("Value");
                         }, guid: guid);
                    }) { Name = html2, DataTemplate = Templates.HtmlWebViewer },
                ],
                attach: (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; }
                )
                { Guid = guid };

            static void buildNetwork(Guid guid)
            {
                var serviceResolver = Globals.Resolver.Resolve<IServiceResolver>(guid.ToString());
                serviceResolver.Connect<ListCollectionViewReturnParam, ListCollectionViewParam>();
                serviceResolver.Connect<PredicateReturnParam, PredicateParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListInParam>();
                serviceResolver.Connect<ListInstanceReturnParam, ListParam>();
                serviceResolver.Connect<SelectionReturnParam, InValueParam>();
                serviceResolver.Connect<OutValueParam, GeneralInNodeParam>();
            }
        }
    }
}