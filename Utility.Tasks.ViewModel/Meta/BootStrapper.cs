using Autofac;
using Utility.Tasks.Model;


namespace Utility.Tasks.ViewModel.Meta
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterViewModels(builder);

            Infrastructure.BootStrapper.Register(builder);
        }

        private static void RegisterViewModels(ContainerBuilder builder)
        {
            builder.RegisterType<FactoryViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<MultiTaskViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<LimitCollection<IProgressState>>().AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}
