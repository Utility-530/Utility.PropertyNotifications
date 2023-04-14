using System.Windows.Forms;
using Abstractions;
using PropertyTrees.Demo.Model;
using PropertyTrees.Infrastructure;
using PropertyTrees.WPF.Demo;
using SoftFluent.Windows.Diagnostics;
using System.Windows;
using Utility.Collections;
using Application = System.Windows.Application;

namespace SoftFluent.Windows.Samples
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            var propertyStore = new WebStore();
            AutoObject.PropertyStore = BaseActivator.PropertyStore = propertyStore;
            Collection.Context = DispatcherTimer.Context = System.Threading.SynchronizationContext.Current;
            BaseActivator.Interfaces = new() { { typeof(IViewModel), typeof(ViewModel) } };
            var window = new Window { Content = new PropertyView { DataContext = new PropertyTrees.Demo.Model.Model() } };
            window.Show();
            var controlWindow = new ControlWindow(propertyStore.Controllable, propertyStore.History);
            SetOnSecondScreen(controlWindow);
            controlWindow.Show();

            base.OnStartup(e);
#if DEBUG
            Tracing.Enable();
#endif
        }

        private void SetOnSecondScreen(Window window)
        {
            Screen s = Screen.AllScreens[1];
            System.Drawing.Rectangle r = s.WorkingArea;
            window.Top = r.Top;
            window.Left = r.Left;
        }

        public class WebStore : PropertyStore
        {
            private HttpRepository store = new();

            protected override IRepository Repository => store;
        }
    }
}