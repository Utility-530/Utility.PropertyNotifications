using DryIoc;
using Splat;
using System.Windows;
using Utility.Interfaces.Exs;
using Utility.Nodify.Generator.Services;
using Utility.Nodify.Transitions.Demo.NewFolder;
using Utility.ServiceLocation;
using MainViewModel = Utility.Nodify.Transitions.Demo.Infrastructure.MainViewModel;
using Utility.Nodify.Transitions.Demo.Infrastructure;

namespace Utility.Nodify.Transitions.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            GlobalRegistration.registerGlobals(Globals.Register);
            initialise(Locator.CurrentMutable);

            register();


            var window = new Window()
            {
                Content = Globals.Resolver.Resolve<MainViewModel>()
            };
            window.Show();
            base.OnStartup(e);
        }

        private void register()
        {
            FactoryOne.build(Globals.Resolver.Resolve<IModelResolver>());
            FactoryOne.connect(Globals.Resolver.Resolve<IServiceResolver>(), Globals.Resolver.Resolve<IModelResolver>());
            FactoryOne.initialise(Globals.Resolver.Resolve<IModelResolver>());
        }

        private void initialise(IMutableDependencyResolver resolver)
        {
            resolver.RegisterLazySingleton(() => new CollectionViewService() { Name = nameof(CollectionViewService) });
            resolver.RegisterLazySingleton<Utility.Nodify.Operations.Infrastructure.INodeSource>(() => new NodeSource() {  });
        }
    }
}
