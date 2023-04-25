using System.Windows.Forms;
using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using SoftFluent.Windows.Diagnostics;
using System.Windows;
using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure.Abstractions;
using System.Threading;
using Utility.Common;
using Autofac;
using System.Reflection;
using Utility.Infrastructure;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            var container = BootStrapper.Build();

            //var propertyStore = repository
            PropertyActivator.Instance.Repository = container.Resolve<IRepository>();
            PropertyActivator.Instance.Interfaces = new() { { typeof(IViewModel), typeof(ViewModel) } };

            AutoObject.Resolver = new Utility.Infrastructure.Resolver(container);
            Collection.Context = BaseObject.Context = SynchronizationContext.Current;

            var window = new Window { Content = new PropertyView { DataContext = new PropertyTrees.Demo.Model.Model() } };
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
            window.Show();
            var controlWindow = container.Resolve<HistoryWindow>();
            controlWindow.Show();
            SetOnLastScreen(controlWindow);

            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(controlWindow, true);

            base.OnStartup(e);
#if DEBUG
            Tracing.Enable();
#endif
        }

        private void SetOnLastScreen(Window window)
        {
            Screen s = Screen.AllScreens[Screen.AllScreens.Length-1];
            System.Drawing.Rectangle r = s.WorkingArea;
            window.Top = r.Top;
            window.Left = r.Left;
        }
    }
}