using Utility.Infrastructure;
using Utility.Observables.NonGeneric;
using Utility.Observables.Generic;
using ReactiveUI;

namespace Utility.PropertyTrees.WPF.Demo;

internal class ModelController : BaseObject
{
    private HUD_Simulator model { get; set; }

    public override Key Key => new(Guids.ModelController, nameof(ModelController), typeof(ModelController));

    public ModelController()
    {
    }

    public void OnNext(StartEvent startEvent)
    {
        if (startEvent.Property.Data is HUD_Simulator rootModel)
        {
            Initialise(rootModel);
        }

    }

    private void Initialise(HUD_Simulator rootModel)
    {
        model = rootModel;
        model.GameModel.ScreenSaver.WhenAnyValue(a => a.JSON)
            .WhereNotNull()
            .Subscribe(json =>
            {
                Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.GameModel.ScreenSaver), json))
                        .Subscribe(a =>
                        {
                        });
            });

        model.GameModel.Leaderboard.WhenAnyValue(a => a.JSON)
            .WhereNotNull()
            .Subscribe(json =>
            {
                Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.GameModel.Leaderboard), json))
                .Subscribe(a =>
                {
                });
            });

        model.GameModel.PrizeWheel.WhenAnyValue(a => a.JSON)
            .WhereNotNull()
            .Subscribe(json =>
            {
                Observe<ClientMessageResponse, ClientMessageRequest>(new(nameof(model.GameModel.PrizeWheel), json))
                .Subscribe(a =>
                {
                });
            });

        model.ServerConnection.WhenAnyValue(a => a.ServerRequest)
            .WhereNotNull()
            .Subscribe(request =>
            {
                Observe<ServerResponse, ServerRequest>(request)
                            .Subscribe(a =>
                            {
                            });
            });
    }

    public void OnNext(ServerEvent serverEvent)
    {
        if (serverEvent.Type == ServerEventType.Open)
        {
            model.ServerConnection.IsConnected = true;
        }
        if (serverEvent.Type == ServerEventType.Close)
        {
            model.ServerConnection.IsConnected = false;
        }
        model.ServerConnection.Events.Add(serverEvent);
    }
}