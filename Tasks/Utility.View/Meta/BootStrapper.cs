using Autofac;
using ReactiveUI;
using Utility.ViewModel;

namespace Utility.View.Meta
{
    public class BootStrapper
    {
        public static void Register(ContainerBuilder builder)
        {
            RegisterViews(builder);
            //RegisterServices(builder);
            Utility.ViewModel.Meta.BootStrapper.Register(builder);
        }

        private static void RegisterViews(ContainerBuilder builder)
        {

            builder.RegisterType<CollectionView>().As<IViewFor<CollectionViewModel>>();
            builder.RegisterType<KeyCollectionView>().As<IViewFor<KeyCollection>>();


        }

        //private static void RegisterServices(ContainerBuilder builder)
        //{
        //    builder.RegisterType<StringTaskItemFactory>().SingleInstance().AsSelf().AsImplementedInterfaces();

        //}
    }
}
