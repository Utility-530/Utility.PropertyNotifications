using Autofac;
using ReactiveUI;
using Splat.Autofac;
using System.Windows;
using System.Reactive.Concurrency;
using System.Threading;
using System.Reactive;
using System.Reactive.Linq;
using Splat;

using static Utility.Tasks.DemoApp.ViewModel.DemoFactoryViewModel;
using Utility.Tasks.DemoApp.ViewModel;
using Utility.Tasks.Model;
using Utility.Tasks.DemoApp.Infrastructure;
using Utility.Tasks.DemoApp.View;
using Utility;
using Utility.Infrastructure;

namespace DemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            builder.Register((c) => new SynchronizationContextScheduler(SynchronizationContext.Current)).As<IScheduler>().SingleInstance();

            // View
            builder.RegisterType<Utility.Tasks.DemoApp.View.FactoryView>().As<IViewFor<DemoFactoryViewModel>>();
            builder.RegisterType<DemoTPLView>().As<IViewFor<DemoTPLViewModel>>();
            builder.RegisterType<DialogCommandView>().As<IViewFor<DialogCommandViewModel>>();
            builder.RegisterType<LoginView>().As<IViewFor<LoginViewModel>>();


            // ViewModel          
            builder.RegisterType<DemoConfigurationViewModel>().RegisterDefault();
            builder.RegisterType<DemoFactoryViewModel>().RegisterDefault();


            builder.RegisterType<DemoTPLViewModel.TPLConfigurationViewModel>().RegisterDefault();
   
            builder.RegisterType<DemoTPLViewModel.TPLMainViewModel>().RegisterDefault();
            builder.RegisterType<DemoTPLViewModel.TPLOutputViewModel>().RegisterDefault();
            builder.RegisterType<DemoTPLViewModel>().RegisterDefault();

            builder.RegisterType<LoginDialogViewModel>().RegisterDefault();
            builder.RegisterType<LoginViewModel>().RegisterDefault();
            builder.Register(c => Observable.Return(new LoginDialogCommandViewModelConfiguration(true))).SingleInstance();
            builder.Register(c => Observable.Empty<CloseRequest>()).SingleInstance();

            // Service
            builder.RegisterType<DemoFactory>().As<IFactory<StringFactoryOutput, Unit>>().SingleInstance();
            builder.RegisterType<WorkerScheduler>().As<ISchedulerWrapper>().SingleInstance();
            builder.RegisterType<Authenticator>().RegisterDefault();


            Utility.Tasks.View.Meta.BootStrapper.Register(builder);
            Utility.Progress.Meta.BootStrapper.Register(builder);


            builder.UseAutofacDependencyResolver();

            //var defaultViewLocator = Locator.Current.GetService<IViewLocator>();
            //Locator.CurrentMutable.RegisterLazySingleton<IViewLocator>(() => new ConventionBasedViewLocator(defaultViewLocator));

        }

        class WorkerScheduler : ISchedulerWrapper
        {
            public IScheduler Scheduler { get; } = RxApp.TaskpoolScheduler;
        }
    }
}
