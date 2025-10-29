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
                    new Model(() => [new CommandModel<OpenSettingsEvent> { Name = Settings }])
                    { 
                         Name = controls,
                         Orientation = Enums.Orientation.Horizontal
                    },
                    new Model(items, attach: node =>
                    {
                        node
                        .WhenReceivedFrom(a => a.Current, includeNulls: false)
                        .Select(current =>
                        {
                            if (current is TypeModel { Type: { } stype })
                            {
                                return stype;
                            }
                            else
                                throw new Exception("£D£");
                        }).Observe<ChangeTypeParam, Type>();
                    })
                    {
                        Name = list,
                        Orientation = Orientation.Vertical,
                        IsChildrenTracked = false
                    }
                ])
                {
                    Name = str,
                    Orientation = Orientation.Vertical
                });

            static IEnumerable<IReadOnlyTree> items()
            {
                foreach (var type in Locator.Current.GetService<IEnumerableFactory<Type>>().Create(null))
                {
                    var pnode = new TypeModel(type) { Name = type.Name, DataTemplate = "HeaderTypeModel" };
                    //pnode.Set(typeModel.Guid, nameof(ModelTypeModel.Guid));
                    yield return pnode;
                }
            }
        }
    }
}
