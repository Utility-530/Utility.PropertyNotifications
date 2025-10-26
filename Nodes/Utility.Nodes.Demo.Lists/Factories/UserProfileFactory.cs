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
using Utility.ServiceLocation;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public IObservable<INodeViewModel> BuildUserProfileRoot(Guid guid, Type type)
        {
            buildNetwork(guid);

            return nodeSource.Create(nameof(BuildUserProfileRoot),
                guid,
                s =>
                new Model(() => [
                     new Model<string>() { Name = search,DataTemplate = "SearchEditor" },
                     new ListModel(type) { Name = list },
                     new EditModel { Name = edit },
                ],               
                (addition) =>
                {
                    if (addition is EditModel { } editModel)
                    {
                        editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { editModel.Value = a; editModel.RaisePropertyChanged(nameof(EditModel.Value)); }, guid: guid);
                    }

                    if (addition is Model<string> { Name: search } searchModel)
                    {
                        searchModel.Observe<FilterParam>(guid);
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
            }
        }
    }
}
