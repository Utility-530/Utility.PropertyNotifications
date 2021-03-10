using ReactiveAsyncWorker.Model;
using ReactiveAsyncWorker.ViewModel;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace ReactiveAsyncWorker.DemoApp.ViewModel
{
    public class DemoFactoryViewModel : ReactiveObject
    {
        public DemoFactoryViewModel(DemoConfigurationViewModel factory2ViewModel, FactoryViewModel factoryViewModel)
        {
            Configuration = factory2ViewModel;
            Main = factoryViewModel;
        }
        
        public DemoConfigurationViewModel Configuration { get; }

        public FactoryViewModel Main { get; }

        public class DemoConfigurationViewModel : ReactiveObject, IObservable<FactoryRequest<Unit>>
        {
            private readonly ReplaySubject<FactoryRequest<Unit>> factoryOrderObserver = new ReplaySubject<FactoryRequest<Unit>>();

            public DemoConfigurationViewModel()
            {
                var create = ReactiveCommand.Create(() =>
                {
                    var request = new FactoryRequest<Unit>(Guid.NewGuid().ToString().Remove(9), default, default);
                    return request;
                });

                var createDelayed = ReactiveCommand.Create(() =>
                {
                    var request = new FactoryRequest<Unit>(Guid.NewGuid().ToString().Remove(9), DateTime.Now.AddSeconds(5),default);
                    return request;
                });

                create.Merge(createDelayed)
                    .Subscribe(a =>
                    factoryOrderObserver.OnNext(a));

                Create = create;
                CreateDelayed = createDelayed;
            }

            public ICommand Create { get; }

            public ICommand CreateDelayed { get; }

            public IDisposable Subscribe(IObserver<FactoryRequest<Unit>> observer)
            {
                return factoryOrderObserver.Subscribe(observer);
            }
        }
        public class DemoFactory : IFactory<StringFactoryOutput, Unit>
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            public StringFactoryOutput Create(Unit args)
            {
                var length = random.Next(4, 10);
                return new StringFactoryOutput(new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()));
            }
        }
    }
}
