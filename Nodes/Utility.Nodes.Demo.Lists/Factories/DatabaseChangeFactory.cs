using System.Collections;
using System.Reactive.Linq;
using Utility.Enums;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.Services;
using Utility.Extensions;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory 
    {
        public IObservable<INodeViewModel> BuildDatabaseChangeRoot(Guid guid, Type type)
        {
            return nodeSource.Create(nameof(BuildDatabaseChangeRoot),
                guid,
                s =>
                new Model(() => [
                     new StringModel(nodeAction: node=>node.DataTemplate = "SearchEditor") { Name = search },
                     //new ListModel(type) { Name = list },
                     //new EditModel { Name = edit },
                ],
                (node) => { node.IsExpanded = true; node.Orientation = Orientation.Vertical; },
                (addition) =>
                {
                    //if (addition is EditModel { } editModel)
                    //{
                    //    editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { editModel.Value = a; editModel.RaisePropertyChanged(nameof(EditModel.Value)); });
                    //}

                    //if (addition is StringModel { Name: search } searchModel)
                    //{
                    //    searchModel.Observe<FilterParam>();
                    //}

                    //if (addition is ListModel { } listModel)
                    //{
                    //    listModel.ReactTo<ListCollectionViewReturnParam>(setAction: (a) => listModel.Collection = (IEnumerable)a);

                    //    listModel.WhenReceivedFrom(a => a.Add, includeNulls: false)
                    //    .Select(a => new Changes.Change(a, null, Changes.Type.Add))
                    //    .Observe<ChangeParam, Changes.Change>();

                    //    listModel.WhenReceivedFrom(a => a.Remove, includeNulls: false)
                    //    .Select(a => new Changes.Change(a, null, Changes.Type.Remove))
                    //    .Observe<ChangeParam, Changes.Change>();

                    //    listModel.Observe<SelectionParam>();

                    //}
                })
                { Name = main });
        }

    }
}
