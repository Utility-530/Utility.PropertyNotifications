using System.Dynamic;
using System.Windows;
using Dynamitey;
using ImpromptuInterface;
using Splat;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.WPF.Controls.Meta;
using Utility.WPF.Demo.ComboBoxes.Infrastructure;

namespace Utility.WPF.Demo.ComboBoxes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.CurrentMutable.RegisterLazySingleton<IContext>(() => Impromptu.ActLike<IContext>(Build<ExpandoObject>.NewObject(UI: SynchronizationContext.Current)));
            Locator.CurrentMutable.RegisterLazySingleton<INodeRoot>(() => new NodeEngine());

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewControl(typeof(ComboBoxUserControl).Assembly)
            }.Show();
            base.OnStartup(e);
        }
    }
}