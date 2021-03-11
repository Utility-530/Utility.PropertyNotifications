using Autofac;
using ReactiveAsyncWorker.Model;
using ReactiveAsyncWorker.ViewModel;
using ReactiveUI;

namespace ReactiveAsyncWorker.Wpf.View.Infrastructure
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterViews(builder);
            //RegisterServices(builder);
            ViewModel.Infrastructure.BootStrapper.Register(builder);
        }

        private static void RegisterViews(ContainerBuilder builder)
        {
            builder.RegisterType<MultiTaskView>().As<IViewFor<MultiTaskViewModel>>();
            builder.RegisterType<CollectionView>().As<IViewFor<CollectionViewModel>>();
            builder.RegisterType<FactoryView>().As<IViewFor<FactoryViewModel>>();
            builder.RegisterType<KeyCollectionView>().As<IViewFor<KeyCollection>>();
            builder.RegisterType<LimitCollectionView>().As<IViewFor<LimitCollection>>();
            builder.RegisterType<ProgressStateView>().As<IViewFor<ProgressState>>();
            builder.RegisterType<ReactiveProcessPairView>().As<IViewFor<ReactiveProcessPair>>();
            builder.RegisterType<ProgressStateSummaryView>().As<IViewFor<ProgressStateSummary>>();
  

        }

        //private static void RegisterServices(ContainerBuilder builder)
        //{
        //    builder.RegisterType<StringTaskItemFactory>().SingleInstance().AsSelf().AsImplementedInterfaces();

        //}
    }
}
