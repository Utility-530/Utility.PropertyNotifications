using Autofac;
using ReactiveUI;
using Splat.Autofac;
using System.Windows;
using ReactiveAsyncWorker.DemoApp.ViewModel;
using System.Reactive.Concurrency;
using System.Threading;
using ReactiveAsyncWorker.Model;
using ReactiveAsyncWorker.Wpf.View;
using ReactiveAsyncWorker.ViewModel;
using static ReactiveAsyncWorker.DemoApp.ViewModel.DemoFactoryViewModel;
using System.Reactive;
using Splat;
using ReactiveAsyncWorker.DemoApp.Infrastructure;
using ReactiveAsyncWorker;

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
            builder.RegisterType<ReactiveAsyncWorker.DemoApp.View.FactoryView>().As<IViewFor<DemoFactoryViewModel>>();  
            builder.RegisterType<ReactiveAsyncWorker.DemoApp.View.DemoTPLView>().As<IViewFor<DemoTPLViewModel>>();  


            // ViewModel          
            builder.RegisterType<DemoConfigurationViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<DemoFactoryViewModel>().SingleInstance().AsSelf().AsImplementedInterfaces();
  

            builder.RegisterType<DemoTPLViewModel.TPLConfigurationViewModel>().AsSelf().SingleInstance().AsImplementedInterfaces();
            builder.RegisterType<DemoTPLViewModel.TPLMainViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DemoTPLViewModel.TPLOutputViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DemoTPLViewModel>().AsSelf().SingleInstance();            
           

            // Service
            builder.RegisterType<DemoFactory>().As<IFactory<StringFactoryOutput, Unit>>().SingleInstance();
            builder.RegisterType<WorkerScheduler>().As<ISchedulerWrapper>().SingleInstance();


            ReactiveAsyncWorker.Wpf.View.Infrastructure.BootStrapper.Register(builder);

            

            builder.UseAutofacDependencyResolver();

            //var defaultViewLocator = Locator.Current.GetService<IViewLocator>();
            //Locator.CurrentMutable.RegisterLazySingleton<IViewLocator>(() => new ConventionBasedViewLocator(defaultViewLocator));
   
        }

        class WorkerScheduler: ISchedulerWrapper
        {
            public IScheduler Scheduler { get; } = RxApp.TaskpoolScheduler;
        }


        //public class Mediator<T> : IObservable<T>, IObserver<T>
        //{

        //    ReplaySubject<T> subject = new ReplaySubject<T>();

        //    public Mediator(IObservable<T> observable)
        //    {
        //        observable.Subscribe(subject.OnNext);
        //    }

        //    public void OnCompleted()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void OnError(Exception error)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public void OnNext(T value)
        //    {
        //        subject.OnNext(value);
        //    }

        //    public IDisposable Subscribe(IObserver<T> observer)
        //    {
        //        return subject.Subscribe(observer);
        //    }
        //}
    }
}
