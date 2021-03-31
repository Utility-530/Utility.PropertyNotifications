using Autofac;
using Utility.Tasks.Model;
using System.Reactive;
using Utility.Tasks.Service;

namespace Utility.Tasks.Infrastructure
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterServices(builder);

        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<DataFlowQueue>().SingleInstance().AsSelf().AsImplementedInterfaces().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<FactoryQueue<StringFactoryOutput, Unit>>().SingleInstance().AsSelf().AsImplementedInterfaces();
            //builder.RegisterType<StringTaskItemFactory>().SingleInstance().AsSelf().AsImplementedInterfaces();
        }
    }
}
