using System.Reactive.Linq;
using Splat;
using Utility.Entities;
using Utility.Entities.Comms;
using Utility.Enums;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Models.Trees;
using Utility.Nodes.Demo.Lists.Services;
using Utility.Nodes.Meta;
using Utility.PropertyNotifications;
using Utility.Trees.Abstractions;
using Utility.ServiceLocation;
using Utility.Models.Diagrams;
using Utility.Meta;
namespace Utility.Nodes.Demo.Lists.Factories
{
    internal partial class NodeMethodFactory : EnumerableMethodFactory
    {
        public INodeViewModel BuildListRoot()
        {
            return
                new Model(() =>
                [
                    new Model(() => [new CommandModel<OpenSettingsEvent> { Name = Settings }])
                    {
                         Name = controls,
                         Orientation = Enums.Orientation.Horizontal,
                         IsExpanded = true,
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
                        IsChildrenTracked = false,
                        IsExpanded = true,
                    },
                    new Model(attach: node =>
                    {
                        node.ReactTo<ComboServiceOutputParam, Changes.Change<INodeViewModel>>(setAction: change =>
                        {
                            switch(change.Type)
                            {
                                case Utility.Changes.Type.Add:
                                    if(change.Value is NodeViewModel _nvm)
                                    {
                                        node.Add(_nvm);
                                    }
                                    break;
                                case Utility.Changes.Type.Remove:
                                    if(change.OldValue is NodeViewModel nvm)
                                    {
                                        node.Remove(nvm);
                                    }
                                    break;
                                case Utility.Changes.Type.Reset:
                                    node.Clear();
                                    break;
                            }
                        });

                        Globals.Events
                        .OfType<OpenSettingsEvent>()
                        .Subscribe(e =>
                        {
                            Locator.Current.GetService<INodeRoot>().Create(nameof(Factories.NodeMethodFactory.BuildSettingsRoot))
                            .Subscribe(model =>
                            {
                                 node.Clear();
                                 node.Add(model);
                            });
                            Globals.Register.Register<IServiceResolver>(() => new ServiceResolver(), Factories.NodeMethodFactory.settingsRootGuid.ToString());
                        });
                    })
                    {
                        Name = slave,
                        IsExpanded = true,
                        Orientation = Orientation.Vertical,
                        IsChildrenTracked = false
                    }
                ])
                {
                    Name = nameof(BuildListRoot),
                    IsExpanded = true,
                    Orientation = Orientation.Horizontal,
                    Guid = listRootGuid
                };

            static IEnumerable<IReadOnlyTree> items()
            {
                foreach (var metaData in Locator.Current.GetService<IEnumerableFactory<EntityMetaData>>().Create(null))
                {
                    var pnode = new TypeModel(metaData.Type) 
                    {
                        Name = metaData.Type.Name, 
                        DataTemplate = "HeaderTypeModel" 
                    };
                    //pnode.Set(typeModel.Guid, nameof(ModelTypeModel.Guid));
                    yield return pnode;
                }
            }
        }
    }
}