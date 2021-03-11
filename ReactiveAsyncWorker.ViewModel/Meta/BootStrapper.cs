using Autofac;
using ReactiveAsyncWorker.Model;
using ReactiveAsyncWorker.ViewModel;
using ReactiveAsyncWorker.ViewModel.Service;


namespace ReactiveAsyncWorker.ViewModel.Infrastructure
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterViewModels(builder);
            RegisterServices(builder);
            ReactiveAsyncWorker.Infrastructure.BootStrapper.Register(builder);
        }

        private static void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<FactoryViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();     
            builder.RegisterType<MultiTaskViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<LimitCollection<ProgressState>>().AsSelf().AsImplementedInterfaces().SingleInstance();
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<StringTaskItemFactory>().SingleInstance().AsSelf().AsImplementedInterfaces();
        }
    }
}
