using LanguageExt.Pipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Netly;
using Netly.Core;
using Netly.Abstract;
using System.Reactive.Subjects;
using System.Reactive;
using Byter;
using System.Reactive.Threading.Tasks;
using System.Diagnostics;
using Utility.Models;
using Utility.Interfaces.Generic;
using Utility.Models.UDP;
using DryIoc;

namespace Utility.Infrastructure
{
    class Constants
    {
        public const string IPAddress = "127.0.0.1";
        public const int Port = 8000;
    }

    public interface ILogger
    {
        Task<Unit> Send(GuidValue guidValue, IObserverIOType observer);
        void Send(IBase[] nodes);
        void Send(IBase node);
    }

    public class DummyLogger : ILogger
    {
        public async Task<Unit> Send(GuidValue guidValue, IObserverIOType observer)
        {
            return await Task.FromResult(Unit.Default);
        }

        public void Send(IBase[] nodes)
        {
         
        }

        public void Send(IBase node)
        {
      
        }
    }

    public class Logger:ILogger
    {
        private readonly IContainer container;
        UdpClient client = new();
        Dictionary<Guid, ReplaySubject<Unit>> observables = new();
        SynchronizationContext synchronizationContext => container.Resolve<SynchronizationContext>();
        public Logger(IContainer container)
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
            this.container = container;
        }

        public Task<Unit> Send(GuidValue guidValue, IObserverIOType observer)
        {
            //var str = JsonSerializer.Serialize(guidValue);
            var guidDto = new GuidDto(guidValue.Node);
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

        public void Send(IBase[] nodes)
        {
            var dtos = nodes.Select(a => new KeyDto(a.Key.Guid, a.Key.Name)).ToArray();
            var serialized = JsonSerializer.Serialize(dtos, typeof(KeyDto[]));
            Writer writer = new();
            writer.Write(serialized);
            var bytes = writer.GetBytes();
            client.ToEvent(nameof(KeyDto), bytes);
        }
        public void Send(IBase node)
        {
            var dto = new KeyDto(node.Key.Guid, node.Key.Name);
            var serialized = JsonSerializer.Serialize(new[] { dto }, typeof(KeyDto[]));
            Writer writer = new();
            writer.Write(serialized);
            var bytes = writer.GetBytes();
            client.ToEvent(nameof(KeyDto), bytes);
        }

    }
}

