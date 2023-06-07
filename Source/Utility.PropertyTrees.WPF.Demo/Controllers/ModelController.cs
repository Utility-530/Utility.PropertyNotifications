using Utility.Infrastructure;
using NetFabric.Hyperlinq;
using Utility.Interfaces.NonGeneric;
using Utility.Observables.NonGeneric;
using Utility.PropertyTrees.WPF.Meta;
using DryIoc;
using Utility.Nodes;
using Utility.Observables.Generic;
using System.Reactive.Disposables;
using Utility.Helpers.Ex;

namespace Utility.PropertyTrees.WPF.Demo;

public interface IModelController : IObservable, IBase
{
    Interfaces.Generic.IObservable<TreeView> TreeView(ValueNode propertyNode, TreeView treeView);

    void OnNext(Request value);
}

internal class ModelController : BaseObject, IModelController
{
    private List<object> messages = new();
    private List<IObserver> observers = new();
    //private ModelViewModel model => container.Resolve<ModelViewModel>();

    public override Key Key => new(Guids.ModelController, nameof(ModelController), typeof(ModelController));

    //public override object? Model => model;

    RootModel model => (RootModel)container.Resolve<RootModelProperty>().Data;


    //ObservableCommand sendLeaderboard, sendPrizewheel, sendScreensaver;
    public ModelController()
    {
        CompositeDisposable composite = new();
        //model.Model = container.Resolve<RootProperty>().Data as RootModel ?? throw new Exception("dfs ooo");
        //model.Server = container.Resolve<ServerProperty>().Data as Server ?? throw new Exception("dfs ppp");


       // sendLeaderboard = new ObservableCommand((observer) =>
       // {
       //     observer.OnNext(false);
       //     var message = JsonSerializer.Serialize(model.Model.GameModel.Leaderboard);
       //     Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.Model.GameModel.Leaderboard), message))
       //     .Subscribe(a =>
       //     {
       //         observer.OnNext(true);
       //     });
       // }, false);
       // sendPrizewheel = new ObservableCommand((observer) =>
       // {
       //     observer.OnNext(false);
       //     var message = JsonSerializer.Serialize(model.Model.GameModel.PrizeWheel);
       //     Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.Model.GameModel.PrizeWheel), message))
       //      .Subscribe(a =>
       //      {
       //          observer.OnNext(true);
       //      });
       // }, false);
       //sendScreensaver = new ObservableCommand((observer) =>
       // {
       //     observer.OnNext(false);
       //     var message = JsonSerializer.Serialize(model.Model.GameModel.ScreenSaver);
       //     Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.Model.GameModel.ScreenSaver), message))
       //      .Subscribe(a =>
       //      {
       //          observer.OnNext(true);
       //      });
       // }, false);
       // model.Connect = new ObservableCommand((observer) =>
       // {
       //     observer.OnNext(false);
       //     Observe<ServerResponse, ServerRequest>(new(model.Model.Server.IP, model.Model.Server.Port))
       //    .Subscribe(a =>
       //    {
       //        observer.OnNext(true);
       //    });
       //// }, true);
       // model.WhenAnyValue(a => a.IsConnected).Subscribe(x =>
       // {
       //     //sendLeaderboard.OnNext(x);
       //     //sendPrizewheel.OnNext(x);
       //     //sendScreensaver.OnNext(x);
       // });
       // model.SendLeaderboard = sendLeaderboard;
       // model.SendPrizewheel = sendPrizewheel;
       // model.SendScreensaver = sendScreensaver;
    }

    public Interfaces.Generic.IObservable<TreeView> TreeView(ValueNode propertyNode, TreeView treeView)
    {
        return Observe<TreeViewResponse, TreeViewRequest>(new TreeViewRequest(treeView, propertyNode))
            .Select(a => a.TreeView);
    }

    public void OnNext(Request value)
    {
        switch (value)
        {
            case ConnectRequest:
                Observe<ServerResponse, ServerRequest>(new(model.Server.IP, model.Server.Port))
               .Subscribe(a =>
               {
               });
                break;
            case RefreshRequest:

                break;
            case ScreensaverRequest:
                {
                    var message = JsonHelper.Serialize(model.GameModel.ScreenSaver);
                    model.JSON = message;
                    Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.GameModel.ScreenSaver), message))
                     .Subscribe(a =>
                     {
                     });
                    break;
                }
            case PrizeWheelRequest:
                {
                    var message = JsonHelper.Serialize(model.GameModel.PrizeWheel);
                    model.JSON = message;
                    Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.GameModel.PrizeWheel), message))
                     .Subscribe(a =>
                     {
                     });
                    break;
                }
            case LeaderboardRequest:
                {
                    var message = JsonHelper.Serialize(model.GameModel.Leaderboard);
                    model.JSON = message;
                    Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.GameModel.Leaderboard), message))
                    .Subscribe(a =>
                    {

                    });
                    break;
                }
        }

        //foreach (var observer in observers)
        //    observer.OnNext(new RefreshEvent());
    }

    public void OnNext(ServerEvent serverEvent)
    {
        if(serverEvent.Type == ServerEventType.Open)
        {
            model.Server.IsConnected = true;
        }
        if (serverEvent.Type == ServerEventType.Close)
        {
            model.Server.IsConnected = false;
        }
        model.Events.Add(serverEvent);
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
