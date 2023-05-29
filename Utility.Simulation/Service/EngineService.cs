//using System;
//using System.Collections.Generic;
//using System.Reactive.Subjects;

//using PlayerDemo.Infrastructure;

//namespace PlayerDemo
//{

//        public class EngineService : IObserver<EngineRecord>
//        {

//            //record TimeSpanRecord(TimeSpan Rate) : EngineRecord;
//            //record TrackRecord(DateTimeRange Track) : EngineRecord;
//            //record HeartBeatRecord(long HeartBeat) : EngineRecord;

//            readonly Subject<EngineRecord> subject = new();
//            //readonly Subject<DateTimeRange> trackSubject = new();
//            //readonly Subject<TimeSpan> timeSpanSubject = new();

//            public EngineService(TimeTrack dateTimeRange)
//            {


//                // .Scan(new Model(), (c, v) =>
//                // {
//                //     var (_, record) = v;
//                //     switch (record)
//                //     {
//                //         case CommandTypeRecord { CommandType: { } commandType } rec:
//                //             switch (commandType)
//                //             {
//                //                 case CommandType.PlayOrPause:
//                //                     c.Enumerator.Current.
//                //                     break;
//                //                 case CommandType.Stop:
//                //                     break;
//                //                 case CommandType.Repeat:
//                //                     break;
//                //                 case CommandType.Forward:
//                //                     c.Enumerator.Current.MoveToStart();
//                //                     if (c.Enumerator.CanMoveNext())
//                //                         c.Enumerator.MoveNext();
//                //                     else
//                //                         c.Enumerator.Reset();
//                //                     break;
//                //                 case CommandType.Backward:
//                //                     if (c.Enumerator.CanMovePrevious())
//                //                         c.Enumerator.MovePrevious();
//                //                     break;
//                //             };
//                //             break;
//                //         case TrackRecord { Track: { } track }:
//                //             //c.Add(track);
//                //             break;
//                //         case TimeSpanRecord { Rate: { } rate }:
//                //             //c.Add(track);
//                //             break;
//                //     }
//                //     return c;
//                // })
//                //.Subscribe();



//            }

//            public void OnCompleted()
//            {
//                throw new NotImplementedException();
//            }

//            public void OnError(Exception error)
//            {
//                throw new NotImplementedException();
//            }

//            public void OnNext(EngineRecord value)
//            {
//                subject.OnNext(value);
//            }

//            //public void OnNext(DateTimeRange value)
//            //{
//            //    trackSubject.OnNext(value);
//            //}

//            //public void OnNext(TimeSpan value)
//            //{
//            //    timeSpanSubject.OnNext(value);
//            //}
//        }
    

//    public class Model
//    {
//        public Model()
//        {
//            Enumerator = new TwoWayEnumerator<TimeTrack>(new List<TimeTrack>());
//        }

//        public TwoWayEnumerator<TimeTrack> Enumerator { get; }

//        public TimeSpan Rate { get; set; }
//    }

//}
