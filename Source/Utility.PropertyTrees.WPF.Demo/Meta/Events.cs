using Netly;
using Utility.Models;

public record ViewModelEvent(string Name, TreeView TreeView);
public record RefreshRequest();


public record ServerRequest(string IP, int Port) : Request;
public record ServerResponse(bool IsInitialised) : Response(IsInitialised);

public record ClientMessageRequest(string Name, string Message) : Request;
public record ClientMessageResponse() : Response(default);

public record ServerEvent : Event;
public record ServerOpenEvent : ServerEvent;
public record ServerCloseEvent : ServerEvent;
public record ServerErrorEvent(Exception Exception) : ServerEvent;
public record ServerEnterEvent(UdpClient Client) : ServerEvent;
public record ServerExitEvent(UdpClient Client) : ServerEvent;
public record ClientMessageEvent(UdpClient Client, ClientData ClientData) : ServerEvent;

public record RefreshEvent : Event;




