using AvalonEditB.Search;
using Newtonsoft.Json;
using Splat;
using System.Collections;
using System.IO;
using System.Reactive.Linq;
using System.Text.Json.Nodes;
using Utility.Enums;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Infrastructure;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Filters;
using Utility.PropertyNotifications;
using Utility.Services;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {

        private readonly INodeSource nodeSource = Current.GetService<INodeSource>();

        public IObservable<INode> BuildEbayRoot(Guid guid, Type type)
        {
            return nodeSource.Create(nameof(BuildEbayRoot), guid, (s) =>
                new Model(() => [
                    new Model(()=>
                    [
                        new CommandModel() { Name = refresh },
                        new StringModel(nodeAction: node=> {node.DataTemplate = "SearchEditor"; node.Title = "Search"; }, attach: searchModel=>{

                                  searchModel.Observe<FilterParam>();
                        }) { Name = search },
                        new StringModel(nodeAction: node=> {node.DataTemplate = "DirectoryEditor"; node.Title = "Base Directory"; }, attach: stringModel=>{
                               stringModel.Observe<BasePathParam>();
                        } ) { Name = directory },
                        new StringModel(nodeAction: node=> {node.DataTemplate = "FilePathEditor"; node.Title = "Index Path"; }, attach: stringModel =>{
                               stringModel.Observe<FilePathParam>();
                        }) { Name = indexPath },
                    ],
                    node=> {node.IsExpanded = true;  node.Orientation = Orientation.Horizontal; },
                    (parent,addition)=>{


                    }){ Name= controllerPath },
                    new ListModel(type, attach: model=>{

                        if(model is not ListModel listModel)
                            throw new Exception("dsdsdddd333");
                        listModel.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a);

                        listModel.WhenReceivedFrom(a => a.Add, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Add))
                        .Observe<ChangeParam, Changes.Change>();

                        listModel.WhenReceivedFrom(a => a.Remove, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Remove))
                        .Observe<ChangeParam, Changes.Change>();

                        listModel.Observe<SelectionParam>();

                    }) { Name = list1 },
                    new EditModel(attach: editModel=>{
                            editModel.WithChangesTo(a => (a as IValue).Value)
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

                                eModel.WithChangesTo(a => a.RelativePath).Observe<FilePathParam, string>();

                            }
                        });

                        editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { (editModel as ISetValue).Value = a; editModel.RaisePropertyChanged(nameof(EditModel.Value)); });

                    }) { Name = edit },
                    new Model<string>(nodeAction: n => n.DataTemplate = "Json", attach: jsonModel=>{

                    jsonModel.ReactTo<FullPathParam>(a =>
                        {
                            var path = Path.Combine(a.ToString(), "data.json");
                            if (!File.Exists(path))
                            {
                                return JsonConvert.SerializeObject(new { Error = "File does not exist: " + path });
                            }
                            var text = File.ReadAllText(path);
                            return text;
                        }, a => jsonModel.Set(a.ToString()));
                        }, raisePropertyCalled:false, raisePropertyReceived:false) { Name = details },
                    new StringModel(nodeAction: n=> n.DataTemplate="Html") { Name = html },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlEditor", attach: stringModel=>{
                              stringModel.ReactTo<RazorFileReturnParam>(setAction: a => stringModel.Set((string)a));
                    }) { Name = html1 },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlWebViewer", attach: rstringModel =>{
                         rstringModel.ReactTo<RazorFileReturnParam>(setAction: a => rstringModel.Set((string)a));
                    }) { Name = html2 },
                ],
                (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; },
                (parent, addition) =>
                {

                }
                )
                { Name = s });
        }
    }
}
