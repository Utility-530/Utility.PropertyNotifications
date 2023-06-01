using Utility.Infrastructure;
using Utility.Commands;
using NetFabric.Hyperlinq;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.NonGeneric;
using Utility.PropertyTrees.Infrastructure;
using Utility.PropertyTrees.WPF.Meta;
using Utility.PropertyTrees.Demo.Model;
using DryIoc;
using Utility.Nodes;
using Utility.Observables.Generic;
using Utility.Collections;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Utility.PropertyTrees.WPF.Demo;

public interface IModelController : IObservable, IBase
{
    Interfaces.Generic.IObservable<TreeView> TreeView(ValueNode propertyNode, TreeView treeView);

    void OnNext(RefreshRequest value);
}

internal class ModelController : BaseObject, IModelController
{
    private List<object> messages = new();
    private List<IObserver> observers = new();
    private ModelViewModel model => container.Resolve<ModelViewModel>();

    public override Key Key => new(Guids.ModelController, nameof(ModelController), typeof(ModelController));

    public override object? Model => model;

    ObservableCommand sendLeaderboard, sendPrizewheel, sendScreensaver;
    public ModelController()
    {
        CompositeDisposable composite = new();
        model.Model = container.Resolve<ModelProperty>().Data as Model ?? throw new Exception("dfs ooo");
        model.Server = container.Resolve<ServerProperty>().Data as Server ?? throw new Exception("dfs ppp");


        sendLeaderboard = new ObservableCommand((observer) =>
        {
            observer.OnNext(false);
            var message = JsonSerializer.Serialize(model.Model.Leaderboard);
            Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.Model.Leaderboard), message))
            .Subscribe(a =>
            {
                observer.OnNext(true);
            });
        }, false);
        sendPrizewheel = new ObservableCommand((observer) =>
        {
            observer.OnNext(false);
            var message = JsonSerializer.Serialize(model.Model.PrizeWheel);
            Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.Model.PrizeWheel), message))
             .Subscribe(a =>
             {
                 observer.OnNext(true);
             });
        }, false);
       sendScreensaver = new ObservableCommand((observer) =>
        {
            observer.OnNext(false);
            var message = JsonSerializer.Serialize(model.Model.ScreenSaver);
            Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.Model.ScreenSaver), message))
             .Subscribe(a =>
             {
                 observer.OnNext(true);
             });
        }, false);
        model.Connect = new ObservableCommand((observer) =>
        {
            observer.OnNext(false);
            Observe<ServerResponse, ServerRequest>(new(model.Server.IP, model.Server.Port))
           .Subscribe(a =>
           {
               observer.OnNext(true);
           });
        }, true);
        model.WhenAnyValue(a => a.IsConnected).Subscribe(x =>
        {
            sendLeaderboard.OnNext(x);
            sendPrizewheel.OnNext(x);
            sendScreensaver.OnNext(x);
        });
        model.SendLeaderboard = sendLeaderboard;
        model.SendPrizewheel = sendPrizewheel;
        model.SendScreensaver = sendScreensaver;
    }

    public Interfaces.Generic.IObservable<TreeView> TreeView(ValueNode propertyNode, TreeView treeView)
    {
        return Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, propertyNode))
            .Select(a => a.TreeView);
    }

    public void OnNext(RefreshRequest value)
    {
        foreach (var observer in observers)
            observer.OnNext(new RefreshEvent());
    }

    public void OnNext(ServerEvent serverEvent)
    {
        Observe<ActivationResponse, ActivationRequest>(new(default, new RootDescriptor(serverEvent), serverEvent, PropertyType.Root))
            .Select(a => a.PropertyNode)
            .Subscribe(newNode =>
            {
                TreeView(newNode, new TreeView())
                    .Select(a => new ViewModelEvent(serverEvent.GetType().Name, a))
                    .Subscribe(a =>
                    {
                        foreach (var observer in observers)
                            observer.OnNext(a);
                    }, () =>
                    {

                    });
            }, () =>
            {

            });
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
