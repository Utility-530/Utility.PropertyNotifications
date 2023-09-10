using Netly;
using Utility.Models;
using Utility.Nodes.Abstractions;
using Utility.PropertyTrees;

public record StartEvent(RootProperty Property):Event;

public record ConnectRequest() : Request;
public record ScreensaverRequest() : Request;
public record PrizeWheelRequest() : Request;
public record LeaderboardRequest() : Request;
public record ServerRequest(string IP, int Port) : Request;
public record ServerResponse(bool IsInitialised) : Response(IsInitialised);

public record ClientMessageRequest(string Name, string Message) : Request;
public record ClientMessageResponse(DateTime DateTime ) : Response(DateTime);


public enum ServerEventType
{
    Open,Close, Error, Exit, Message,
    Enter,
    Data
}

public record ServerEvent(ServerEventType Type) : Event
{
        public Exception? Exception { get; init; }
        public UdpClient? Client { get; init; }
        public ClientData? Data { get; init; }
}

//public record ServerEvent(ServerEventType Type) : Event;
//public record ServerOpenEvent() : ServerEvent(ServerEventType.Open);
//public record ServerCloseEvent() : ServerEvent(ServerEventType.Close);
//public record ServerErrorEvent(Exception Exception) : ServerEvent(ServerEventType.Error);
//public record ServerEnterEvent(UdpClient Client) : ServerEvent(ServerEventType.Enter);
//public record ServerExitEvent(UdpClient Client) : ServerEvent(ServerEventType.Exit);
//public record ServerMessageEvent(UdpClient Client, ClientData ClientData) : ServerEvent(ServerEventType.Message);
internal static class ServerEventsFactory
{
    public static ServerEvent ErrorEvent(Exception exception)
    {
        return new ServerEvent(ServerEventType.Error) { Exception = exception };
    }   
    
    public static ServerEvent OpenEvent()
    {
        return new ServerEvent(ServerEventType.Open) { };
    }
    public static ServerEvent CloseEvent()
    {
        return new ServerEvent(ServerEventType.Close) {  };
    }
    public static ServerEvent EnterEvent(UdpClient Client)
    {
        return new ServerEvent(ServerEventType.Enter) { Client = Client };
    }
    public static ServerEvent ExitEvent(UdpClient Client)
    {
        return new ServerEvent(ServerEventType.Exit) {  Client = Client };
    }
    public static ServerEvent MessageEvent(UdpClient Client, ClientData ClientData)
    {
        return new ServerEvent(ServerEventType.Message) { Client = Client, Data = ClientData };
    }
}
