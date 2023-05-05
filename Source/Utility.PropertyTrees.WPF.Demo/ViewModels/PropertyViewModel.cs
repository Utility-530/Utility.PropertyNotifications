using System;
using System.Windows.Controls;
using System.Reactive.Threading.Tasks;
using Utility.Infrastructure;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.Commands;
using Utility.PropertyTrees.Demo.Model;
using static Utility.PropertyTrees.WPF.Demo.UdpServerController;
using System.Text.Json;
using NetFabric.Hyperlinq;
using System.Reactive.Linq;
using System.Collections.Generic;
using Utility.Interfaces.NonGeneric;
using System.Collections;
using Utility.Observables.NonGeneric;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class PropertyViewModel : BaseObject, IObservable
    {
        Guid Guid = Guid.Parse("e123dab9-9022-4649-9dc4-754d492dd5a6");
        private ICommand sendScreensaver, sendPrizewheel, sendLeaderboard, connect;

        public override Models.Key Key => new(Guid, nameof(PropertyViewModel), typeof(PropertyViewModel));

        public PropertyViewModel()
        {
            sendLeaderboard = new Command(() =>
            {
                var message = JsonSerializer.Serialize(Model.Leaderboard);
                Broadcast(new ClientMessageRequest(message));
            });
            sendPrizewheel = new Command(() =>
            {
                var message = JsonSerializer.Serialize(Model.PrizeWheel);
                Broadcast(new ClientMessageRequest(message));
            });
            sendScreensaver = new Command(() =>
            {
                var message = JsonSerializer.Serialize(Model.ScreenSaver);
                Broadcast(new ClientMessageRequest(message));
            });

            connect = new Command(() =>
            {
                Broadcast(new ServerRequest(Server.IP, Server.Port));
            });
        }

        public Model Model { get; } = new();
        public Server Server { get; } = new();

        public ICommand SendLeaderboard => sendLeaderboard;
        public ICommand SendPrizewheel => sendPrizewheel;
        public ICommand SendScreensaver => sendScreensaver;
        public ICommand Connect => connect;

        public IEnumerable<IObserver> Observers => throw new NotImplementedException();

        public IObservable<TreeView> TreeView(PropertyNode propertyNode)
        {
            return Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(new TreeView(), propertyNode))
                .Select(a => a.TreeView);
        }

        List<object> messages = new();

        public override void OnNext(object value)
        {
            if (value is ClientResponseEvent message)
            {
                var newNode = new PropertyNode(Guid.NewGuid())
                {
                    Data = message
                };
                TreeView(newNode)
                    .Select(a => new ViewModelEvent(message.ClientData.Header, a))
                .Subscribe(a =>
                {

                    foreach (var observer in observers)
                        observer.OnNext(message);
                });
                return;
            }
            base.OnNext(value);
        }

        public List<IObserver> observers = new();

        public IDisposable Subscribe(IObserver observer)
        {
            return new Disposer(observers, observer);
        }

        public IEnumerator GetEnumerator()
        {
            return messages.GetEnumerator();
        }
    }

    public record ViewModelEvent(string Name, TreeView TreeView);
}