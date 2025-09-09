using DryIoc;
using Splat;
using System.Windows;
using Utility.Interfaces.Exs;
using Utility.Nodify.Generator.Services;
using Utility.ServiceLocation;
using MainViewModel = Utility.Nodify.Demo.Infrastructure.MainViewModel;
using Utility.Nodify.Demo.Infrastructure;

namespace Utility.Nodify.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Registration.registerGlobals(Globals.Register);
            Registration.initialise(Locator.CurrentMutable);
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

    }
}
