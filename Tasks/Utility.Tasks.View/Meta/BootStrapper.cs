using Autofac;
using Utility.Tasks.ViewModel;
using ReactiveUI;
using Utility.Tasks.Model;
using Utility.ViewModel;

namespace Utility.Tasks.View.Meta
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterViews(builder);
            //RegisterServices(builder);
            Utility.Tasks.ViewModel.Meta.BootStrapper.Register(builder);
        }

        private static void RegisterViews(ContainerBuilder builder)
        {
            builder.RegisterType<MultiTaskView>().As<IViewFor<MultiTaskViewModel>>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<FactoryView>().As<IViewFor<FactoryViewModel>>();

            builder.RegisterType<LimitCollectionView>().As<IViewFor<LimitCollection>>();
            builder.RegisterType<ProgressStateView>().As<IViewFor<ProgressState>>();
            builder.RegisterType<ReactiveProcessPairView>().As<IViewFor<ReactiveProcessPair>>();
            builder.RegisterType<ProgressStateSummaryView>().As<IViewFor<ProgressStateSummary>>();
            builder.RegisterType<ProgressView>().As<IViewFor<ProgressViewModel>>();


        }

        //private static void RegisterServices(ContainerBuilder builder)
        //{
        //    builder.RegisterType<StringTaskItemFactory>().SingleInstance().AsSelf().AsImplementedInterfaces();

        //}
    }
}
