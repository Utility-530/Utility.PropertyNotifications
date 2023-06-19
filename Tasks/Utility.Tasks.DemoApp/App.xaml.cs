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
using Utility.Models;
using Unit = System.Reactive.Unit;
using Utility.View;

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
            builder.RegisterType<FactoryView>().As<IViewFor<DemoFactoryViewModel>>();
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
            //Utility.Progressions.Meta.BootStrapper.Register(builder);
            Utility.Progressions.View.Meta.BootStrapper.Register(builder);


            // Creates and sets the Autofac resolver as the Locator
            var autofacResolver = builder.UseAutofacDependencyResolver();

            // Register the resolver in Autofac so it can be later resolved
            builder.RegisterInstance(autofacResolver);

            // Initialize ReactiveUI components
            autofacResolver.InitializeReactiveUI();

            var defaultViewLocator = Locator.Current.GetService<ReactiveUI.IViewLocator>();
            Locator.CurrentMutable.RegisterLazySingleton<IViewLocator>(() => new ConventionBasedViewLocator(defaultViewLocator, new[] { typeof(CollectionView) }, typeof(DefaultView)));


            var container = builder.Build();

            //https://github.com/reactiveui/splat/blob/main/src/Splat.Autofac/README.md
            // If you need to override any service (such as the ViewLocator), register it after InitializeReactiveUI
            // https://autofaccn.readthedocs.io/en/latest/register/registration.html#default-registrations
            // builder.RegisterType<MyCustomViewLocator>().As<IViewLocator>().SingleInstance();
            //Set Autofac Locator's lifetime after the ContainerBuilder has been built

            //var autofacResolver = container.Resolve<AutofacDependencyResolver>();


            // Set a lifetime scope (either the root or any of the child ones) to Autofac resolver
            // This is needed, because the previous steps did not Update the ContainerBuilder since they became immutable in Autofac 5+
            // https://github.com/autofac/Autofac/issues/811
            autofacResolver.SetLifetimeScope(container);


        }

        class WorkerScheduler : ISchedulerWrapper
        {
            public IScheduler Scheduler { get; } = RxApp.TaskpoolScheduler;
        }
    }
}
