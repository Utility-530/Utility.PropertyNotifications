using Utility.Infrastructure;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;
using ReactiveUI;
using Utility.Helpers;
using Utility.PropertyTrees.Services;
using Utility.Helpers.Ex;

namespace Utility.PropertyTrees.WPF.Demo;

internal class ModelController : BaseObject
{
    private RootModel model { get; set; }

    public override Key Key => new(Guids.ModelController, nameof(ModelController), typeof(ModelController));

    public ModelController()
    {
    }

    public void OnNext(StartEvent startEvent)
    {
        if (startEvent.Property.Data is RootModel rootModel)
        {
            Initialise(rootModel);
        }
    }

    private void Initialise(RootModel rootModel)
    {
        model = rootModel;
        model.HUD_Simulator.GameModel.ScreenSaver.WhenAnyValue(a => a.JSON)
            .WhereNotNull()
            .Subscribe(json =>
            {
                Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.HUD_Simulator.GameModel.ScreenSaver), json))
                        .Subscribe(a =>
                        {
                        });
            });

        model.HUD_Simulator.GameModel.Leaderboard.WhenAnyValue(a => a.JSON)
            .WhereNotNull()
            .Subscribe(json =>
            {
                Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.HUD_Simulator.GameModel.Leaderboard), json))
                .Subscribe(a =>
                {
                });
            });

        model.HUD_Simulator.GameModel.PrizeWheel.WhenAnyValue(a => a.JSON)
            .WhereNotNull()
            .Subscribe(json =>
            {
                Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.HUD_Simulator.GameModel.PrizeWheel), json))
                .Subscribe(a =>
                {
                });
            });

        model.HUD_Simulator.ServerConnection.WhenAnyValue(a => a.ServerRequest)
            .WhereNotNull()
            .Subscribe(request =>
            {
                Observe<ServerResponse, ServerRequest>(request)
                .Subscribe(a =>
                {
                });
            });

        model.ViewModels.WhenAnyValue(a => a.Key)
            .WhereNotNull()
        .Subscribe(a =>
        {
            var key = new Key(model.ViewModels.Guid, model.ViewModels.Name, Type.GetType(model.ViewModels.Type));
            foreach (var x in model.ViewModels.Collection)
                this.Observe<SetViewModelResponse, SetViewModelRequest>(new(key, x))
                .Subscribe(response =>
                {

                });
        });

        model.WhenAnyValue(a => a.LastRefresh)
            .WhereNotDefault()
            .Subscribe(a =>
            {
                Send(new RefreshRequest(a));
            });
    }


    public void OnNext(ClickChange selectionChange)
    {
        if (selectionChange is { Node: PropertyBase { Key: Key Key } node })
        {
            if (node.SelfAndAncestors().Any(a => a.Name == "ViewModels"))
                return;

            model.ViewModels.Guid = Key.Guid;
            model.ViewModels.Type = Key.Type.AsString();
            model.ViewModels.Name = Key.Name;

            this.Observe<GetViewModelResponse, GetViewModelRequest>(new(Key))
                .Subscribe(response =>
                {
                    if (model.ViewModels.Collection.Any())
                        model.ViewModels.Collection.Clear();
                    foreach (ViewModel viewmodel in response.ViewModels)
                        model.ViewModels.Collection.Add(viewmodel);
                });
        }
    }

    public void OnNext(ServerEvent serverEvent)
    {
        if (serverEvent.Type == ServerEventType.Open)
        {
            model.HUD_Simulator.ServerConnection.IsConnected = true;
        }
        if (serverEvent.Type == ServerEventType.Close)
        {
            model.HUD_Simulator.ServerConnection.IsConnected = false;
        }
        model.HUD_Simulator.ServerConnection.Events.Add(serverEvent);
    }
}