using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Utility.Simulation.Infrastructure;

namespace Utility.Simulation.Service
{
    public enum End { Start, Finish }

    public struct Skip
    {
        public Skip(int count)
        {
            Count = count; End = null;
        }
        public Skip(End end)
        {
            End = end; Count = null;
        }

        public End? End { get; }
        public int? Count { get; }
    }

    public class Engine : IObserver<Engine.Instruction>, IObservable<Engine.Position>
    {
        public record Instruction(DateTime? Target = null, double? Rate = null, TimeSpan? Time = null, Skip? Skip = null);
        public record Position(bool IsRunning, DateTime Current);

        static readonly TimeSpan intervalRate = TimeSpan.FromSeconds(0.1);
        private readonly Subject<Instruction> subject = new();
        private readonly IObservable<Position> positionSubject;

        public enum State
        {
            Running, Stopped
        }

        public Engine(TimeTrack Range)
        {
            Observable
                .Interval(intervalRate, RxApp.TaskpoolScheduler)
                .Subscribe(a => subject.OnNext(default(Instruction)));

            positionSubject = subject
                .Scan(new Model(), (a, b) =>
                 {
                     if (Range.TimeRemaining == TimeSpan.Zero)
                         Range.MoveToStart();


                     if (b != null)
                     {

                     }
                     if (b == null)
                     {
                         if (a.Counter != null)
                         {
                             a.Counter.Reduce();
                             if (a.Counter.Remaining < TimeSpan.Zero)
                                 return a;
                         }

                         if (ChangeRate(true, Range, a.Rate) == false)
                         {
                             a.Reset();
                             return a;
                         }
                         else
                         {
                             a.IsRunning = true;
                         }
                     }

                     var be = b switch
                     {
                         (DateTime start, null, _, _) => ChangePosition(true, Range, start),
                         (DateTime start, double rate, _, _) => ChangeRate(ChangePosition(true, Range, start), Range, rate),
                         (_, double rate, _, Skip skip) => ChangeRate(Skip(true, Range, skip), Range, rate),
                         (null, double rate, _, _) => ChangeRate(true, Range, rate),
                         null => true,
                         _ => throw new ArgumentOutOfRangeException("FSDfs  xx")
                     };
                     if (be == false)
                     {
                         a.Reset();
                         return a;
                     }
                     else
                     {
                         a.IsRunning = true;
                     }
                     if (b?.Rate.HasValue ?? false)
                     {
                         a.Rate = b.Rate.Value;
                     }

                     if (b?.Time.HasValue ?? false)
                     {
                         a.Counter = new Counter(b.Time.Value, intervalRate);
                     }

                     return a;
                 })
                .Select(a => new Position(a.IsRunning, Range.Current));


            bool ChangePosition(bool valid, TimeTrack range, DateTime dateTime)
            {
                if (valid == false)
                    return false;
                var diff = dateTime - Range.Current;
                switch (Range.CanMove(diff))
                {
                    case true:
                        {
                            Range.Move(diff);
                            return true;
                        }
                    case false:
                        {
                            Range.MoveToEnd();
                            return false;
                        }
                    case null:
                        return false;
                }
            }
            bool ChangeRate(bool valid, TimeTrack range, double rate)
            {
                if (valid == false)
                    return false;
                var diff = rate * intervalRate;
                switch (Range.CanMove(diff))
                {
                    case true:
                        {
                            Range.Move(diff);
                            return true;
                        }
                    case false:
                        {
                            Range.MoveToEnd();
                            return false;
                        }
                    case null:
                        return false;
                }
            }
            bool Skip(bool valid, TimeTrack range, Skip skip)
            {
                if (valid == false)
                    return false;
                if (skip.End.HasValue)
                {
                    if (skip.End.Value == End.Start)
                        Range.MoveToStart();
                    else if (skip.End.Value == End.Finish)
                        Range.MoveToEnd();
                }
                else
                    Range.Move(skip.Count.Value);
                return true;
            }
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Instruction value)
        {
            subject.OnNext(value);
        }

        //public void Dispose()
        //{
        //    disposable.Dispose();
        //    subject.Dispose();
        //}

        public IDisposable Subscribe(IObserver<Position> observer)
        {
            return positionSubject.Subscribe(observer);
        }

        public class Model
        {
            public Counter Counter { get; set; }

            public double Rate { get; set; }

            public bool IsRunning { get; set; }

            public void Reset()
            {
                Counter = null;
                Rate = 0;
                IsRunning = false;
            }
        }

        public class Counter
        {
            public Counter(TimeSpan timeSpan, TimeSpan reduction)
            {
                Remaining = timeSpan;
                Reduction = reduction;
            }

            public TimeSpan Remaining { get; private set; }

            public TimeSpan Reduction { get; }

            public int Count { get; private set; }

            public void Reduce()
            {
                Remaining -= Reduction;
                Count++;
            }
        }
    }

}
