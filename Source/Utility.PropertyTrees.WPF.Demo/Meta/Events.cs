using Netly;
using Utility.Models;

public record ViewModelEvent(string Name, TreeView TreeView);



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

}
public record ServerEvent2(ServerEventType Type) : ServerEvent(Type)
{
    public Exception? Exception { get; init; }
    public UdpClient? Client { get; init; }
    public ClientData? Data { get; init; }
}
//public record ServerErrorEvent(Exception Exception) : ServerEvent(ServerEventType.Open);
//public record ServerEnterEvent(UdpClient Client) : ServerEvent(ServerEventType.Open);
//public record ServerExitEvent(UdpClient Client) : ServerEvent   ;
//public record ClientMessageEvent(UdpClient Client, ClientData ClientData) : ServerEvent(ServerEventType.Open);

public record RefreshEvent : Event;




