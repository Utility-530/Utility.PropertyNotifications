using System.Collections;
using System.Reactive.Linq;
using System.Windows;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Models;
using Utility.Models.Templates;
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
        public INodeViewModel BuildUserProfileRoot(Guid guid, Type type)
        {
            buildNetwork(guid);

            return 
                new Model(() => [
                     new Model<string>() { Name = search,DataTemplate = Templates.SearchEditor },
                     new ListModel(type) { Name = list },
                     new Model {
                        Name = edit,
                        DataTemplate = Templates.EditTemplate,
                        ShouldValueBeTracked = false
                    }, 
                ],
                (addition) =>
                {
                    if (addition is Model { Name:edit } editModel)
                    {
                        editModel.ReactTo<SelectionReturnParam>(setAction: (a) => { editModel.Value = a; editModel.RaisePropertyChanged(nameof(Model.Value)); }, guid: guid);
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
                { Guid = guid };

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