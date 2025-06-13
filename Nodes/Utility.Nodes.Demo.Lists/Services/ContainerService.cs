using Splat;
using Utility.Nodes.WPF;

namespace Utility.Nodes.Demo.Lists.Services
{
    internal class ContainerService
    {
        public ContainerService()
        {
            var container = Locator.Current.GetService<ContainerViewModel>();
            Locator.Current
                .GetService<IObservable<ViewModel>>()
                .Subscribe(a =>
                {
                    container.Selected = a;
                    container.Children.Add(a);
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
        }

    }
}
