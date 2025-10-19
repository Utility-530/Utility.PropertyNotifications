using System.Reactive.Linq;
using Utility.Nodes.Meta;
using Utility.Interfaces.Exs;
using Splat;
using Utility.PropertyNotifications;
using Utility.Nodes.Demo.Editor;
using Utility.Repos;
using Utility.Models.Trees;
using Utility.Interfaces.Generic;
using Utility.ServiceLocation;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class ComboService
    {
        public ComboService()
        {
            Locator.Current.GetService<IObservableIndex<INodeViewModel>>()[nameof(NodeMethodFactory.BuildComboRoot)]
                .Subscribe(node =>
                {
                    IViewModelTree current = null;
                    node.WhenReceivedFrom(a => a.Current, includeNulls: false)
                    .Subscribe(_current =>
                    {
                        if (_current is DataFileModel { FilePath: { } filePath, Guid: { } guid } data)
                        {
                            if (Locator.Current.GetService<SlaveViewModel>(guid.ToString()) is null)
                            {
                                Splat.Locator.CurrentMutable.Register<SlaveViewModel>(() => new SlaveViewModel(data), guid.ToString());
                            }
                            else
                                Locator.Current.GetService<SlaveViewModel>()?.Dispose();

                            var containerViewModel = Locator.Current.GetService<ContainerViewModel>();
                            containerViewModel.Slave = Locator.Current.GetService<SlaveViewModel>(guid.ToString());
                            containerViewModel.RaisePropertyChanged(nameof(ContainerViewModel.Slave));
                            current = _current;
                        }
                    });
                });
        }
    }
}
