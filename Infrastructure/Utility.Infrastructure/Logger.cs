using System.Diagnostics;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using Byter;
using Netly;
using Netly.Core;
using Utility.Collections;
using Utility.Commands;
using Utility.Models.UDP;

namespace Utility.Infrastructure
{
    internal class Constants
    {
        public const string IPAddress = "127.0.0.1";
        public const int Port = 8000;
    }

    public interface ILogger
    {
        Task<Unit> Send(GuidValue guidValue, IObserverIOType observer);

        void Send(BaseObject[] nodes);

        void Send(BaseObject node);

        Task Add(IObserverIOType node);

        Task Remove(IObserverIOType node);
    }

    public class DummyLogger : ILogger
    {
        private Queue<Subject<Unit>> _subjects = new();

        public DummyLogger()
        {
            Command = new ObservableCommand(b =>
            {
                if (_subjects.Any())
                {
                    var sub = _subjects.Dequeue();
                    sub.OnNext(Unit.Default);
                    sub.OnCompleted();
                }
            });
        }

        public ICommand Command { get; }

        public Collection Collection { get; } = new();

        public Task Add(IObserverIOType node)
        {
            //node.Observers.CollectionChanged += Observers_CollectionChanged;
            Collection.Add(node);
            return Task.CompletedTask;
        }

        private void Observers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
            }
            //if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            //{
            //}
        }

        public Task Remove(IObserverIOType node)
        {
            Collection.Remove(node);
            return Task.CompletedTask;
        }

        public async Task<Unit> Send(GuidValue guidValue, IObserverIOType observer)
        {
            var subject = new Subject<Unit>();
            _subjects.Enqueue(subject);
            return await subject.ToTask(Globals.UI);
        }

        public void Send(BaseObject[] nodes)
        {
            foreach (var node in nodes)
                Collection.Add(node);
        }

        public void Send(BaseObject node)
        {
            Collection.Add(node);
        }
    }

    public class Logger : ILogger
    {
        private UdpClient client = new();
        private Dictionary<Guid, ReplaySubject<Unit>> observables = new();
        private SynchronizationContext synchronizationContext => Globals.UI;

        public Logger()
        {
            var host = new Host(Constants.IPAddress, Constants.Port);

            client.OnOpen(() =>
            {
                Debug.WriteLine($"[OPEN]: {host}");

                // send data to server
                client.ToData(NE.GetBytes("hello server"));

                //// send event to server
                client.ToEvent("ping", NE.GetBytes("ping..."));
            });

            client.OnClose(() =>
            {
                Debug.WriteLine($"[CLOSE]: {host}");
            });

            client.OnError((e) =>
            {
                Debug.WriteLine($"[ERROR]: {e}");
            });

            client.OnData((data) =>
            {
                var reader = new Reader(data);
                var guid = reader.Read<string>();
                if (guid == default)
                    return;

                observables[reader.Read<Guid>()].OnNext(Unit.Default);
                //Console.WriteLine($"[DATA]: {NE.GetString(data)}");
            });

            client.OnEvent((name, data) =>
            {
                synchronizationContext.Post((a) =>
                {
                    if (name == "event")
                    {
                        //  Debug.WriteLine($"[EVENT] -> {name}: {NE.GetString(data)}");
                        var reader = new Reader(data);
                        var guid = reader.Read<string>();
                        if (guid == default)
                            return;
                        var guid_ = Guid.Parse(guid);
                        if (observables.ContainsKey(guid_))
                        {
                            observables[guid_].OnNext(Unit.Default);
                            observables[guid_].OnCompleted();
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }
                }, default);
            });

            client.OnModify((socket) =>
            {
                //socket.NoDelay = true;
            });

            client.Open(host);
        }

        public Task<Unit> Send(GuidValue guidValue, IObserverIOType observer)
        {
            //var str = JsonSerializer.Serialize(guidValue);
            var guidDto = new GuidDto(guidValue.Source);
            var serialized = JsonSerializer.Serialize(guidDto);
            var dto = new TypeDto(observer.InType.Name, observer.OutType.Name);
            var serialized2 = JsonSerializer.Serialize(dto);

            //var message = new Message(Guid.NewGuid(), x);
            var guid = Guid.NewGuid();
            var subject = new ReplaySubject<Unit>(1);
            observables[guid] = subject;

            Writer writer = new();
            writer.Write(guid.ToString());
            writer.Write(serialized);

            writer.Write(serialized2);

            var bytes = writer.GetBytes();

            var task = observables[guid].ToTask();
            client.ToEvent(nameof(GuidDto), bytes);
            return task;
        }

        public void Send(BaseObject[] nodes)
        {
            var dtos = nodes.Select(a => new KeyDto(a.Key.Guid, a.Key.Name)).ToArray();
            var serialized = JsonSerializer.Serialize(dtos, typeof(KeyDto[]));
            Writer writer = new();
            writer.Write(serialized);
            var bytes = writer.GetBytes();
            client.ToEvent(nameof(KeyDto), bytes);
        }

        public void Send(BaseObject node)
        {
            var dto = new KeyDto(node.Key.Guid, node.Key.Name);
            var serialized = JsonSerializer.Serialize(new[] { dto }, typeof(KeyDto[]));
            Writer writer = new();
            writer.Write(serialized);
            var bytes = writer.GetBytes();
            client.ToEvent(nameof(KeyDto), bytes);
        }

        public Task Add(IObserverIOType node)
        {
            throw new NotImplementedException();
        }

        public Task Remove(IObserverIOType node)
        {
            throw new NotImplementedException();
        }
    }
}