using System.Reactive.Linq;
using Utility.Nodes.Filters;
using Utility.Interfaces.Exs;
using Splat;
using Utility.PropertyNotifications;
using Utility.Nodes.Demo.Editor;
using Utility.Repos;
using Utility.Models.Trees;
using Utility.Interfaces.Generic;
using Utility.ServiceLocation;

namespace Utility.Nodes.Demo.Filters.Services
{
    public class ComboService
    {
        public ComboService()
        {
            Locator.Current.GetService<IObservableIndex<INode>>()[nameof(NodeMethodFactory.BuildComboRoot)]
                .Subscribe(node =>
                {
                    node.WithChangesTo(a => a.Current)
                    .Subscribe(a =>
                    {
                        if (a is INode { Data: DataFileModel { FilePath: { } filePath } data })
                        {
                            if (Locator.Current.GetService<SlaveViewModel>() is null)
                            {
                                Splat.Locator.CurrentMutable.Register<SlaveViewModel>(() => new SlaveViewModel());
                            }
                            else
                                Locator.Current.GetService<SlaveViewModel>()?.Dispose();

                            Globals.Register.UnregisterAll<ITreeRepository>();
                            Globals.Register.Register<ITreeRepository>(() => new TreeRepository(filePath));

                            Globals.Resolver.Resolve<INodeSource>().Dispose();
                            Globals.Register.UnregisterAll<INodeSource>();
                            Globals.Register.Register<INodeSource>(() => new NodeEngine());

                            if (Locator.Current.GetService<ParserService>() is { } parserService)
                            {
                                parserService.Dispose();
                                Locator.CurrentMutable.UnregisterAll<ParserService>();
                            }
                            Locator.CurrentMutable.RegisterConstant(new ParserService());
                            Locator.Current.GetService<ContainerViewModel>().RaisePropertyChanged(nameof(ContainerViewModel.Slave));
                        }
                    });
                });
        }
    }
}
