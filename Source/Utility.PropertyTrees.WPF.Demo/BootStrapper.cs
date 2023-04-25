using Utility.PropertyTrees.Abstractions;
using Utility.PropertyTrees.Infrastructure;
using Utility.Infrastructure.Abstractions;
using Autofac;
using Utility.Infrastructure;
using Utility.Interfaces.NonGeneric;
using Utility.Helpers.Ex;

namespace Utility.PropertyTrees.WPF.Demo
{
    public partial class App
    {
        public class BootStrapper
        {
            public static IContainer Build()
            {
                var builder = new ContainerBuilder();

                // Register individual components
                builder.RegisterType<History>().AsSelf().As<IHistory>().SingleInstance();
                builder.RegisterType<Playback>().AsSelf().As<IPlayback>().SingleInstance();
                builder.RegisterType<HttpRepository>().AsSelf().As<IRepository>().SingleInstance();
                builder.RegisterType<HistoryWindow>().AsSelf().As<IObserver>().SingleInstance();
                builder.RegisterType<PropertyStore>().AsSelf().As<IPropertyStore>().SingleInstance();
                builder.RegisterSelf();
                // Scan an assembly for components
                //builder.RegisterAssemblyTypes(myAssembly)
                //       .Where(t => t.Name.EndsWith("Repository"))
                //       .AsImplementedInterfaces();

                var container = builder.Build();
                return container;
            }
        }
    }
}