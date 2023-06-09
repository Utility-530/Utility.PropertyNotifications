using Utility.Infrastructure;
using Utility.Observables.NonGeneric;
using DryIoc;
using Utility.Observables.Generic;
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
        model = startEvent.Property.Data as RootModel ?? throw new Exception("vd fdfdwsw2222");
    }

    public void OnNext(Utility.Models.Request value)
    {
        switch (value)
        {
            case ConnectRequest:
                Observe<ServerResponse, ServerRequest>(new(model.Server.IP, model.Server.Port))
                .Subscribe(a =>
                {
                });
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
                    var x = container.Resolve<RootModelProperty>();
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
    }


    public void OnNext(ServerEvent serverEvent)
    {
        if (serverEvent.Type == ServerEventType.Open)
        {
            model.Server.IsConnected = true;
        }
        if (serverEvent.Type == ServerEventType.Close)
        {
            model.Server.IsConnected = false;
        }
        model.Events.Add(serverEvent);
    }
}
