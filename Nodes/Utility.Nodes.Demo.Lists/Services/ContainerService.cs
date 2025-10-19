using Splat;
using System.Reactive.Linq;
using Utility.Common.Models;
using Utility.Entities.Comms;
using Utility.Extensions;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Meta;
using Utility.Models;
using Utility.Models.Diagrams;
using Utility.Nodes.Meta;
using Utility.Observables.Generic;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Services.Meta;
using Utility.Trees;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal record ChangeTypeParam() : Param<ContainerService>(nameof(ContainerService.ChangeType), "type");

    internal class ContainerService
    {
        private readonly ContainerModel container = Current.GetService<ContainerModel>();

        public ContainerService()
        {

            var existing = container.Children.Cast<NodeViewModel>().FirstOrDefault(vc => vc.Name == nameof(Factories.NodeMethodFactory.BuildListRoot));
            if (existing == null)
            {
                //var newItem = Locator.Current.GetService<MasterViewModel>();
                MethodCache.Instance[nameof(Factories.NodeMethodFactory.BuildListRoot)]
                    .Subscribe(update);
            }
            else
            {
                container.Current = existing;
            }

            Globals.Events
                .OfType<OpenSettingsEvent>()
                .Subscribe(e =>
                {
                    Locator.Current.GetService<MethodCache>().Get(nameof(Factories.NodeMethodFactory.BuildSettingsRoot))
                        .Subscribe(model =>
                        {
                            update(model);
                        });
                    Globals.Register.Register<IServiceResolver>(() => new ServiceResolver(), Factories.NodeMethodFactory.settingsRootGuid.ToString());
                });
        }

        public void ChangeType(Type type)
        {
            var factory = Locator.Current.GetService<IFactory<EntityMetaData>>();
            var metaData = factory.Create(type);
            Globals.Register.Register<IServiceResolver>(() => new ServiceResolver(), metaData.Guid.ToString());
            Observable.Return(type).Observe<InstanceTypeParam, Type>(metaData.Guid);

            Locator.Current.GetService<MethodCache>()
                .Get(metaData.TransformationMethod, metaData.Guid, [type])
                .Subscribe(update);
        }

        private void update(Interfaces.Exs.Diagrams.INodeViewModel x)
        {
            x.SetIsSelected(true);
            container.Current = x;
            if (container.Contains(x) == false)
                container.Add(x);
            container.RaisePropertyChanged(nameof(Current));
        }
    }
}