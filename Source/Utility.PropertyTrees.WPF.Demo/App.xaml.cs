using System.Windows.Forms;
using SoftFluent.Windows.Diagnostics;
using System.Windows;
using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure;
using System.Threading;
using DryIoc;
using Utility.Graph.Shapes;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
           
            SQLitePCL.Batteries.Init();
            Collection.Context = BaseObject.Context = SynchronizationContext.Current?? throw new System.Exception("sd w3w");

            var container = new LightBootStrapper().Build();

            BaseObject.Resolver = new Utility.Infrastructure.Resolver(container);

            var window = new Window { Content = new MainView() {  } };
            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(window, true);
            window.Show();

            //ModernWpf.Controls.Primitives.WindowHelper.SetUseModernWindowStyle(controlWindow, true);

            base.OnStartup(e);

#if DEBUG
            Tracing.Enable();
#endif
        }


    }



}