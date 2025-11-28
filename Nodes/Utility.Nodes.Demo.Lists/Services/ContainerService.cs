using System.Reactive.Linq;
using Splat;
using Utility.Entities.Comms;
using Utility.Extensions;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Exs.Diagrams;
using Utility.Interfaces.Generic;
using Utility.Meta;
using Utility.Models.Diagrams;
using Utility.Nodes.Meta;
using Utility.Observables.Generic;
using Utility.ServiceLocation;
using Utility.Services;
using Utility.Services.Meta;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal record ChangeTypeParam() : Param<ContainerService>(nameof(ContainerService.ChangeType), "type");
    public record ComboServiceOutputParam() : Param<ContainerService>(nameof(ContainerService.ChangeType));

    public class ContainerService
    {

        public static IObservable<Changes.Change<INodeViewModel>> ChangeType(Type type)
        {
            var factory = Locator.Current.GetService<IFactory<EntityMetaData>>();
            var metaData = factory.Create(type);
            Globals.Register.Register<IServiceResolver>(() => new ServiceResolver(), metaData.Guid.ToString());
            Observable.Return(type).Observe<InstanceTypeParam, Type>(metaData.Guid);

            return Observable.Create<Changes.Change<INodeViewModel>>(observer =>
            {
                return Globals.Resolver.Resolve<INodeRoot>()
                .Create(metaData.TransformationMethod)
                .Subscribe(a =>
                {
                    observer.OnNext(Changes.Change<INodeViewModel>.Reset());
                    observer.OnNext(Changes.Change<INodeViewModel>.Add(a));

                }, () => observer.OnCompleted());
            });
        }
    }
}
