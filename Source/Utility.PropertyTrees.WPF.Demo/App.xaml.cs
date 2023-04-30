using System.Windows.Forms;
using SoftFluent.Windows.Diagnostics;
using System.Windows;
using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure;
using System.Threading;
using DryIoc;
using Utility.GraphShapes;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();
            Collection.Context = Utility.Infrastructure.Resolver.Context = SynchronizationContext.Current?? throw new System.Exception("sd w3w");

            var container = new LightBootStrapper().Build();

            BaseObject.Resolver = new Utility.Infrastructure.Resolver(container);

            var window = new Window { Content = new PropertyView(container) { DataContext = new PropertyTrees.Demo.Model.Model() } };
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
            window.Show();
            var controlWindow = new Window { Content = container.Resolve<HistoryViewModel>(), WindowState = WindowState.Maximized };
            controlWindow.Show();
            SetOnLastScreen(controlWindow);

            new Window { Content = new UserControl1(container) }.Show();
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(controlWindow, true);

            base.OnStartup(e);
            //

#if DEBUG
            Tracing.Enable();
#endif
        }

        private void SetOnLastScreen(Window window)
        {
            Screen s = Screen.AllScreens[Screen.AllScreens.Length - 1];
            System.Drawing.Rectangle r = s.WorkingArea;
            window.Top = r.Top;
            window.Left = r.Left;
        }
    }



}