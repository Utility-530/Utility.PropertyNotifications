using Splat;
using System;
using System.Windows;
using Utility.Interfaces.Exs;
using Utility.ServiceLocation;
using MainViewModel = Utility.Nodify.Demo.Infrastructure.MainViewModel;
using Utility.Nodify.Demo.Infrastructure;
using Utility.WPF.Helpers;

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

            Registration.initialise(Locator.CurrentMutable);
            Registration.registerGlobals(Globals.Register);
            FactoryOne.connect(Globals.Resolver.Resolve<IServiceResolver>());

            var window = new Window()
            {
                Content = Globals.Resolver.Resolve<MainViewModel>()
            };
            window.ToLeft();
            window.Show();
            var mainWindow = new Window()
            {
                ContentTemplate = this.FindResource("ContainerTemplate") as DataTemplate
            };

            Globals.Resolver
                .Resolve<INodeRoot>()
                .Create(nameof(FactoryOne.BuildContainer))
                .Subscribe(node =>
                {
                    mainWindow.Content = node;
                    //shadowWindow.Content = node;
                });
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}
