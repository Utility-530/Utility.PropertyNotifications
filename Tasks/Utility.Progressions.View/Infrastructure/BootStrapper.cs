using Autofac;
using ReactiveUI;
using System;

namespace Utility.Progressions.View.Meta
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<MultiProgressViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();
            containerBuilder.RegisterType<MultiProgressView>().As<IViewFor<MultiProgressViewModel>>();
            containerBuilder.RegisterType<ProgressView>().As<IViewFor<ProgressViewModel>>();
        }
    }
}
