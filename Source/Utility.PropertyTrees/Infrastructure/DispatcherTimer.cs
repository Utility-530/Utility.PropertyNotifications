//using Utility.PropertyTrees.Abstractions;
//using Utility.PropertyTrees.WPF.Demo;
//using System.Reactive.Subjects;
//using Utility.Enums;

//namespace Utility.PropertyTrees.Infrastructure
//{
//    public record Step(DateTime Date, Direction Direction);

//    public class DispatcherTimer : IObserver<ControlType>, IObservable<Step>
//    {
//        private Subject<Step> subject = new();
//        private Direction direction;

//        public static SynchronizationContext Context { get; set; }
//        public Direction Direction => direction;

//        public System.Timers.Timer Timer { get; set; } = new(TimeSpan.FromSeconds(0.1));

//        public DispatcherTimer()
//        {
//            Timer.Elapsed += Timer_Elapsed;
//        }

//        private void Start()
//        {
//            Timer.Start();
//        }

//        private void Stop()
//        {
//            Timer.Stop();
//        }

//        private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
//        {
//            Context.Post(a => subject.OnNext(new Step(e.SignalTime, Direction)), null);
//        }

//        public IDisposable Subscribe(IObserver<Step> observer)
//        {
//            return subject.Subscribe(observer);
//        }

//        public void OnNext(ControlType value)
//        {
//            switch (value)
//            {
//                case ControlType.Pause:
//                    Stop();
//                    break;

//                case ControlType.Play:
//                    direction = Direction.Forward;
//                    Start();
//                    break;

//                case ControlType.Forward:
//                    Stop();
//                    direction = Direction.Forward;
//                    subject.OnNext(new Step(DateTime.Now, Direction));
//                    break;

//                case ControlType.Back:
//                    Stop();
//                    direction = Direction.Forward;
//                    subject.OnNext(new Step(DateTime.Now, Direction));
//                    break;
//            }
//        }

//        public void OnCompleted()
//        {
//            throw new NotImplementedException();
//        }

//        public void OnError(Exception error)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}