using Splat;
using System;
using System.Reactive.Linq;
using Utility.Attributes;
using Utility.Entities;
using Utility.Entities.Comms;
using Utility.Extensions;
using Utility.Helpers.Reflection;
using Utility.Interfaces.Exs;
using Utility.Models;
using Utility.Nodes.Meta;
using Utility.Observables.Generic;
using Utility.ServiceLocation;
using Utility.Services;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal record ChangeTypeParam() : MethodParameter<ContainerService>(nameof(ContainerService.ChangeType), "modelType");

    internal class ContainerService
    {
        private readonly ContainerModel container = Current.GetService<ContainerModel>();

        static readonly Guid settingsGuid = new("ae276135-6a94-48ab-909a-00cadd79c4bc");

        public ContainerService()
        {

            var existing = container.Children.Cast<NodeViewModel>().FirstOrDefault(vc => vc.Name == nameof(Factories.NodeMethodFactory.BuildListRoot));
            if (existing == null)
            {
                //var newItem = Locator.Current.GetService<MasterViewModel>();
                MethodCache.Instance[nameof(Factories.NodeMethodFactory.BuildListRoot)]
                    .Subscribe(root =>
                    {
                        root.SetIsSelected(true);
                        container.Add(root);
                        container.Selected = root;
                    });
            }
            else
            {
                container.Selected = existing;
            }

            Globals.Events
                .OfType<OpenSettingsEvent>()
                .Subscribe(e =>
                {
                    Locator.Current.GetService<MethodCache>().Get(nameof(Factories.NodeMethodFactory.BuildSettingsRoot))
                        .Subscribe(model =>
                        {
                            container.Selected = model;
                            if (container.Contains(model) == false)
                                container.Add(model);
                        });
                    Globals.Register.Register<IServiceResolver>(() => new ServiceResolver(), settingsGuid.ToString());
                });
        }

        public void ChangeType(ModelType modelType)
        {
            if (modelType.Type.TryGetAttribute<ModelAttribute>(out var att) == false)
                throw new Exception("33f $$");

            Globals.Register.Register<IServiceResolver>(() => new ServiceResolver(), att.Guid.ToString());
            Observable.Return(modelType.Type).Observe<InstanceTypeParam, Type>(att.Guid);

            Locator.Current.GetService<MethodCache>()
                .Get(transformMethod(modelType.Type), att.Guid, [modelType.Type])
                .Subscribe(x =>
                {         

                    container.Selected = x;
                    container.Add(x);
                });

            static string? transformMethod(Type type)
            {
                return type.
                    TryGetAttribute<ModelAttribute>(out var att) ?
                    att.TransformMethod :
                    nameof(Factories.NodeMethodFactory.BuildUserProfileRoot);
            }
        }

    }
}