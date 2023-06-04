using System.Windows.Forms;
using UniversalConverter.Diagnostics;
using System.Windows;
using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure;
using System.Threading;
using DryIoc;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {           
            SQLitePCL.Batteries.Init();

            Collection.Context = BaseObject.Context = SynchronizationContext.Current?? throw new System.Exception("sd w3w");

            var container = new BootStrapper().Build();

            var resolver = new Utility.Infrastructure.Resolver(container);
            BaseObject.Resolver = resolver;
            resolver.Initialise();

            var window = new Window { Content = new MainView(container) {  } };
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