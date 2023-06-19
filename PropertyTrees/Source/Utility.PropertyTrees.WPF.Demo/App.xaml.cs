using UniversalConverter.Diagnostics;
using Utility.Collections;
using Application = System.Windows.Application;
using Utility.Infrastructure;
using DryIoc;
using Resolver = Utility.PropertyTrees.Services.Resolver;


namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            SQLitePCL.Batteries.Init();

            Collection.Context = BaseObject.Context = SynchronizationContext.Current ?? throw new System.Exception("sd w3w");

            var container = BootStrapper.Build();

            var resolver = new Resolver(container);
            BaseObject.Resolver = resolver;
            resolver.Initialise();
            resolver.Send(new GuidValue(new StartEvent(container.Resolve<RootModelProperty>())));
            //resolver.Send(new GuidValue(new StartEvent(container.Resolve<RootViewModelsProperty>())));


            //var window2 = new Window { Content = container.Resolve<ILogger>() };
            //window2.Show();

            base.OnStartup(e);

#if DEBUG
            Tracing.Enable();
#endif
        }
    }


}