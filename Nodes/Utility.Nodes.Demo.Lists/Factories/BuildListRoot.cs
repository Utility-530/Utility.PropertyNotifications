using Splat;
using System.Reactive.Linq;
using Utility.Entities;
using Utility.Entities.Comms;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;

namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {  
        public IObservable<INodeViewModel> BuildListRoot()
        {
            return nodeSource.Create(nameof(BuildListRoot),
                listRootGuid,
                str =>

                new Model(() =>
                [
                     new Model(() => [new CommandModel<OpenSettingsEvent> { Name = Settings }],
                     n => { n.IsExpanded = true; n.Orientation = Enums.Orientation.Horizontal; })
                     { Name = controls },

                    new Model(Items, node =>
                    {
                        node.IsExpanded = true;
                        node.Orientation = Orientation.Vertical;
                    }, 
                    attach: node =>
                    {
                        node
                        .WhenReceivedFrom(a => a.Current, includeNulls: false)
                        .Select(current =>
                        {
                            if (current is ModelTypeModel { Value: ModelType { Type: { } stype } value } data)
                            {
                                return value;
                            }
                            else
                                throw new Exception("£D£");
                        }).Observe<ChangeTypeParam, ModelType>();
                    })
                    {
                        Name = list,
                        IsExpanded = true
                    }
                ])
                {
                    Name = str,
                    IsExpanded = true,
                    Orientation = Orientation.Vertical
                });

            static IEnumerable<IReadOnlyTree> Items()
            {
                foreach (var typeModel in Locator.Current.GetService<IEnumerableFactory<ModelType>>().Create(null))
                {
                    var pnode = new ModelTypeModel { Name = typeModel.Alias, Value = typeModel };
                    //pnode.Set(typeModel.Guid, nameof(ModelTypeModel.Guid));
                    yield return pnode;
                }
            }
        }
    }
}
