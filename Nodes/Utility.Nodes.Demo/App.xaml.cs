using Splat;
using System.Threading;
using System.Windows;
using Utility.Collections;
using Utility.Interfaces.Exs;
using Utility.Nodes.Demo.Infrastructure;
//using Utility.Repos;
using Utility.WPF.Templates;

namespace Utility.Nodes.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Collection.Context = SynchronizationContext.Current;
            //Locator.CurrentMutable.RegisterLazySingleton<ITreeRepository>(() => new InMemoryTreeRepository());
            Locator.CurrentMutable.RegisterLazySingleton<System.Windows.Controls.DataTemplateSelector>(() => CustomDataTemplateSelector.Instance);
            Locator.CurrentMutable.RegisterLazySingleton(() =>
            {
                return Resolver.Instance
                            .Register<TopViewModel, BreadcrumbsViewModel>()
                            .Register<BreadcrumbsViewModel, RootViewModel>()
                            .Register<BreadcrumbsViewModel, DescendantsViewModel>();
            });
            var window = new Window { Content = new DemoRootNode() };
            window.Show();
        }
    }
}
