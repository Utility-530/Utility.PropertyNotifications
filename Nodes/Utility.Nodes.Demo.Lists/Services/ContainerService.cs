using Splat;
using Utility.Attributes;
using Utility.Helpers.Reflection;
using Utility.Models;

namespace Utility.Nodes.Demo.Lists.Services
{

    internal record ChangeTypeParam() : MethodParameter<ContainerService>(nameof(ContainerService.ChangeType), "type");

    internal class ContainerService
    {
        private readonly ContainerViewModel container = Current.GetService<ContainerViewModel>();

        public ContainerService()
        {

            var existing = container.Children.FirstOrDefault(vc => vc is MasterViewModel);
            if (existing == null)
            {
                var newItem = Locator.Current.GetService<MasterViewModel>();
                newItem.IsSelected = true;
                container.Children.Add(newItem);
                container.Selected = newItem;
            }
            else
            {
                container.Selected = existing;
            }
        }

        public void ChangeType(Type type)
        {
            if (type.TryGetAttribute<ModelAttribute>(out var att) == false)
                throw new Exception("33f $$");
            var x = new TreeViewModel(transformMethod(type), att.Guid, type);
            container.Selected = x;
            container.Children.Add(x);

            static string? transformMethod(Type type)
            {
                return type.
                    TryGetAttribute<ModelAttribute>(out var att) ?
                    att.TransformMethod :
                    nameof(NodeMethodFactory.BuildUserProfileRoot);
            }
        }

    }
}