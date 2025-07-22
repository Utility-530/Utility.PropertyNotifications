using Newtonsoft.Json;
using Splat;
using System.Collections;
using System.IO;
using System.Reactive.Linq;
using Utility.Enums;
using Utility.Interfaces.Exs;
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
            return nodeSource.Create(nameof(BuildEbayRoot),
                guid,
                s => new Node(s) { IsExpanded = true, Orientation = Orientation.Horizontal },
                s =>
                new Model(() => [
                    new Model(()=>
                    [
                        new CommandModel() { Name = refresh },
                        new StringModel(nodeAction: node=> {node.DataTemplate = "SearchEditor"; node.Title = "Search"; }) { Name = search },
                        new StringModel(nodeAction: node=> {node.DataTemplate = "DirectoryEditor"; node.Title = "Base Directory"; }) { Name = directory },
                        new StringModel(nodeAction: node=> {node.DataTemplate = "FilePathEditor"; node.Title = "Index Path"; }) { Name = indexPath },
                    ],
                    node=> {node.IsExpanded = true;  node.Orientation = Orientation.Horizontal; },
                    (parent,addition)=>{

                        if (addition.Data is StringModel { Name: directory } _stringModel)
                        {
                            _stringModel.Observe<BasePathParam>();
                        }
                        else if (addition.Data is StringModel { Name: search } searchModel)
                        {
                            searchModel.Observe<FilterParam>();
                        }
                        else if (addition.Data is StringModel { Name: indexPath } __stringModel)
                        {
                            __stringModel.WithChangesTo(a => a.Value).Observe<FilePathParam, string?>();
                        }

                    }){ Name= controllerPath },
                    new ListModel(type) { Name = list1 },
                    new EditModel { Name = edit },
                    new Model<string>(nodeAction: n => n.DataTemplate = "Json", raisePropertyCalled:false, raisePropertyReceived:false) { Name = details },
                    new HtmlModel { Name = html },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlEditor") { Name = html1 },
                    new ReadOnlyStringModel(nodeAction: node=> node.DataTemplate = "HtmlWebViewer") { Name = html2 },
                ],
                (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; },
                (parent, addition) =>
                {
                    if (addition.Data is EditModel { } editModel)
                    {
                        editModel.WithChangesTo(a => a.Value)
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

                        editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { editModel.Value = a; editModel.RaisePropertyChanged(nameof(EditModel.Value)); });
                    }
                    else if (addition.Data is ListModel { } listModel)
                    {

                        listModel.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a);

                        listModel.WhenReceivedFrom(a => a.Add, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Add))
                        .Observe<ChangeParam, Changes.Change>();

                        listModel.WhenReceivedFrom(a => a.Remove, includeNulls: false)
                        .Select(a => new Changes.Change(a, null, Changes.Type.Remove))
                        .Observe<ChangeParam, Changes.Change>();

                        listModel.Observe<SelectionParam>();
                    }
                    else if (addition.Data is Model<string> { Name: details } jsonModel)
                    {
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
                        return;
                    }
                    else if (addition.Data is StringModel { } stringModel)
                    {
                        stringModel.ReactTo<RazorFileReturnParam>(setAction: a => stringModel.Set((string)a));
                    }
                    else if (addition.Data is ReadOnlyStringModel { } rstringModel)
                    {
                        rstringModel.ReactTo<RazorFileReturnParam>(setAction: a => rstringModel.Set((string)a));
                    }
                })
                { Name = main });
        }
    }
}
