//using System;
//using System.IO;
//using System.Reactive;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using Newtonsoft.Json;
//using ReactiveUI;
//using Splat;

//namespace Utility.WPF.SuspensionDriver
//{
//    public class NewtonsoftJsonSuspensionDriver : ISuspensionDriver, IEnableLogger
//    {
//        private readonly string stateFilePath;

//        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
//        {
//            TypeNameHandling = TypeNameHandling.All
//        };

//        public NewtonsoftJsonSuspensionDriver(string stateFilePath) => this.stateFilePath = stateFilePath;

//        public IObservable<Unit> InvalidateState()
//        {
//            if (File.Exists(stateFilePath))
//                File.Delete(stateFilePath);
//            return Observable.Return(Unit.Default);
//        }

//        public IObservable<object> LoadState()
//        {
//            try
//            {
//                if (File.Exists(stateFilePath))
//                {
//                    var lines = File.ReadAllText(stateFilePath);
//                    var state = JsonConvert.DeserializeObject<object>(lines, settings);
//                    return state == null ? Observable.Return(new object()) : Observable.Return(state);
//                }
//            }
//            catch (Exception e)
//            {
//                this.Log().Write(e, "Error Loading State", typeof(NewtonsoftJsonSuspensionDriver), LogLevel.Error);
//            }

//            return Observable.Return(new object());
//        }

//        public IObservable<Unit> SaveState(object state)
//        {
//            try
//            {
//                File.WriteAllText(stateFilePath, JsonConvert.SerializeObject(state, Formatting.Indented, settings));
//            }
//            catch (InvalidOperationException e)
//            {
//                RxApp.MainThreadScheduler.Schedule<object?>(default, (a, o) =>
//                 {
//                     File.WriteAllText(stateFilePath, JsonConvert.SerializeObject(state, Formatting.Indented, settings));
//                     return Disposable.Empty;
//                 });
//                this.Log().Write(e, "Error Saving State", typeof(NewtonsoftJsonSuspensionDriver), LogLevel.Warn);
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            return Observable.Return(Unit.Default);
//        }
//    }
//}