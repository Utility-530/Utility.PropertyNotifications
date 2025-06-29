using Splat;
using Utility.Attributes;
using Utility.Entities;
using Utility.Models.Trees;
using Utility.Nodes.Filters;
using Utility.PropertyNotifications;
using Utility.Services;
using Utility.Helpers.Reflection;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal class ContainerService
    {
        public ContainerService()
        {
            var container = Locator.Current.GetService<ContainerViewModel>();

            Locator.Current.GetService<MethodCache>()
             .Get(nameof(Utility.Nodes.Filters.NodeMethodFactory.BuildListRoot))
             .Subscribe(node =>
             {
                 node
                 .WithChangesTo(a => a.Current)
                 .Subscribe(a =>
                 {
                     if (a.Data is ModelTypeModel { Value.Type: { } stype } data)
                     {
                         var type = Type.GetType(stype);
                         if (type.TryGetAttribute<ModelAttribute>(out var att) == false)
                             throw new Exception("33f $$");


                         Locator.Current.GetService<CollectionCreationService>().OnNext(new TypeValue(type));

                         var x = new TreeViewModel(transformMethod(type), att.Guid, type);
                         container.Selected = x;
                         container.Children.Add(x);
                     }
                 });
             });

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