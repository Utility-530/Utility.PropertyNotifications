using Autofac;
using ReactiveAsyncWorker.Model;
using System.Reactive;

namespace ReactiveAsyncWorker.Infrastructure
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterServices(builder);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<StringDataFlowQueue>().SingleInstance().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<FactoryQueue<StringFactoryOutput, Unit>>().SingleInstance().AsSelf().AsImplementedInterfaces();
        }
    }
}
