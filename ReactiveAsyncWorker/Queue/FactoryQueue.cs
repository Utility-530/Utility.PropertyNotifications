
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveAsyncWorker.Model;

namespace ReactiveAsyncWorker
{
    public class FactoryQueue<T, TArgs> : 
        IObserver<FactoryRequest<TArgs>>,
        IObservable<FactoryWrapper<T>>, 
        IObservable<FactoryOrder>
        where T : FactoryOutput
    {
        private readonly ISubject<FactoryWrapper<T>> subject = new ReplaySubject<FactoryWrapper<T>>();
        private readonly ISubject<FactoryRequest<TArgs>> commands = new Subject<FactoryRequest<TArgs>>();
        private readonly ISubject<FactoryOrder> factoryOrders = new Subject<FactoryOrder>();

        public FactoryQueue(IFactory<T,TArgs> factory, IObservable<FactoryRequest<TArgs>> orders)
        {
            orders.Subscribe(commands);

            var timer = new Timer<TimedValue<FactoryRequest<TArgs>>>();
            timer.Subscribe(a =>
            {
                var creation = factory.Create(a.Value.Arguments);
                var order = new FactoryOrder(a.Value.Key, FactoryStatus.Created, a.Finish, a.Start);
                factoryOrders.OnNext(order);
                subject.OnNext(new FactoryWrapper<T>(order, creation));
            });

            commands.Subscribe(a =>
            {
                if (a.ScheduledTime.HasValue)
                {
                    factoryOrders.OnNext(new FactoryOrder(a.Key, FactoryStatus.Scheduled, a.ScheduledTime.Value, DateTime.Now));
                    timer.OnNext(new TimedValue<FactoryRequest<TArgs>>(a, DateTime.Now, a.ScheduledTime.Value));
                }
                else
                {
                    var dtn = DateTime.Now;
                    factoryOrders.OnNext(new FactoryOrder(a.Key, FactoryStatus.Set, dtn));
                    var creation = factory.Create(a.Arguments);
                    var order = new FactoryOrder(a.Key, FactoryStatus.Created, dtn, DateTime.Now);
                    factoryOrders.OnNext(order);
                    subject.OnNext(new FactoryWrapper<T>(order, creation));
                }
            });
        }

        //void Cancel()
        //{
        //    _tokenSource.Cancel();
        //    // _block.Complete();
        //}

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(FactoryRequest<TArgs> value)
        {
            commands.OnNext(value);
        }


        public IDisposable Subscribe(IObserver<FactoryWrapper<T>> observer)
        {
            return subject.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<FactoryOrder> observer)
        {
            return factoryOrders.Subscribe(observer);
        }
    }
}


