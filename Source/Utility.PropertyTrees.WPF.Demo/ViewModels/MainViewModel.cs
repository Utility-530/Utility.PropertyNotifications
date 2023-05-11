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
using Utility.PropertyTrees.Infrastructure;
using Utility.PropertyTrees.WPF.Demo.Views;

namespace Utility.PropertyTrees.WPF.Demo
{
    public class MainViewModel : BaseObject, IObservable
    {
        Guid Guid = Guid.Parse("e123dab9-9022-4649-9dc4-754d492dd5a6");
        private readonly Command sendScreensaver, sendPrizewheel, sendLeaderboard, connect;
        List<object> messages = new();
        List<IObserver> observers = new();

        public override Models.Key Key => new(Guid, nameof(MainViewModel), typeof(MainViewModel));

        public MainViewModel()
        {
            sendLeaderboard = new Command(() =>
            {
                var message = JsonSerializer.Serialize(Model.Leaderboard);
                Broadcast(new ClientMessageRequest(nameof(Model.Leaderboard), message));
            });
            sendPrizewheel = new Command(() =>
            {
                var message = JsonSerializer.Serialize(Model.PrizeWheel);
                Broadcast(new ClientMessageRequest(nameof(Model.PrizeWheel), message));
            });
            sendScreensaver = new Command(() =>
            {
                var message = JsonSerializer.Serialize(Model.ScreenSaver);
                Broadcast(new ClientMessageRequest(nameof(Model.ScreenSaver), message));
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

        public IObservable<TreeView> TreeView(PropertyNode propertyNode, TreeView treeView)
        {
            return Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, propertyNode))
                .Select(a => a.TreeView);
        }

        public override bool OnNext(object value)
        {
            if(value is RefreshRequest)
            {
                foreach (var observer in observers)
                    observer.OnNext(new RefreshEvent());
            }
            if (value is ServerEvent)
            {
                Observe<PropertyNode, ActivationRequest>(new(null, new RootDescriptor(value), value, PropertyType.Root))
                    .Subscribe(newNode =>
                    {
                        TreeView(newNode, new TreeView())
                        .Select(a => new ViewModelEvent(value.GetType().Name, a))
                        .Subscribe(a =>
                        {
                            foreach (var observer in observers)
                                observer.OnNext(a);

                        });
                    });
                return true;
            }
            return base.OnNext(value);
        }

        public IEnumerable<IObserver> Observers => observers;

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

    public record RefreshRequest();
}