
// Example udp client instance
using Byter;
using Netly;
using Netly.Core;
using Utility.Infrastructure;

internal sealed class UdpServerController : BaseObject
{
    private UdpServer? server = new ();

    public override Key Key => new(Guids.UdpServer, nameof(UdpServerController), typeof(UdpServerController));

    public IObservable<ServerResponse> OnNext(ServerRequest serverRequest)
    {
        if (server.Host?.IPEndPoint.Address.ToString() != serverRequest.IP || server.Host?.Port != serverRequest.Port)
        {
            OnNewRequest(serverRequest);
            return Return(new ServerResponse(true));
        }
        return Return(new ServerResponse(false));

        void OnNewRequest(ServerRequest serverRequest)
        {

            var serverHost = new Host(serverRequest.IP, serverRequest.Port);
            //var serverhost = new Host("127.0.0.1", 8000);

            server.OnOpen(() =>
            {
                // connection opened: server start listen client
                //this.OnNext(new ServerOpenEvent());

                Context.Post((a) => this.OnNext(new ServerEvent(ServerEventType.Open)), null);
            });

            server.OnClose(() =>
            {
                // connection closed: server stop listen client
                //this.OnNext(new ServerCloseEvent());
                Context.Post((a) => this.OnNext(new ServerEvent(ServerEventType.Open)), null);
            });

            server.OnError((exception) =>
            {
                Context.Post((a) => this.OnNext(new ServerEvent2(ServerEventType.Error) { Exception = exception}), null);
                // error on open connection
                //this.OnNext(new ServerErrorEvent(exception));
            });

            server.OnEnter((client) =>
            {
                // client connected: connection accepted
                //this.OnNext(new ServerEnterEvent(client));
                this.OnNext(new ServerEvent(ServerEventType.Enter));
            });

            server.OnExit((client) =>
            {
                // client disconnected: connection closed
                //this.OnNext(new ServerExitEvent(client));
                this.OnNext(new ServerEvent2(ServerEventType.Exit) { Client = client});

            });

            server.OnData((client, data) =>
            {
                // buffer/data received: {client: client instance} {data: buffer/data received} 
                //Broadcast(new ServerMessageReceivedEvent(client, new data));
                Reader reader = new(data);
                var output = reader.Read<string>();
                output = reader.Read<string>();
                //throw new Exception("898cdd w");
                //this.OnNext(new ServerEvent());
                Context.Post((a) => this.OnNext(new ServerEvent(ServerEventType.Data)), null);
            });

            server.OnEvent((client, name, data) =>
            {
                Reader reader = new(data);
                var output = reader.Read<string>();
                output = reader.Read<string>();
                //var type = TypeConverter.Instance.ToType(output);
                //var serialised = JsonSerializer.Deserialize(output, type);
                //this.OnNext(new ClientMessageEvent(client, new ClientData(name, output)));
                //Context.Post((a) => this.OnNext(new ServerEvent2(ServerEventType.Message) { Client = client, Data = new ClientData(name, output)}), null);
                Context.Post((a) => this.OnNext(new ServerEvent(ServerEventType.Message)), null);
            });

            // open connection
            server.Open(serverHost);

        }
    }

    public IObservable<ClientMessageResponse> OnNext(ClientMessageRequest request)
    {
        if (server is null)
        {
            throw new Exception("sdfg 090900");
        }
        //Check for exit conditions
        //line = Console.ReadLine();
        using Writer w = new();
        w.Write("8000");
        w.Write(false);
        w.Write(request.Name);
        w.Write(request.Message);
        server. ToEvent("chat", w.GetBytes());
        return Return(new ClientMessageResponse(DateTime.Now));
    }
}


public class ClientData
{
    public ClientData(string header, string message)
    {
        Header = header;
        Message = message;
    }
    public ClientData() { }

    public string Header { get; set; }
    public string Message { get; set; }
}
//public record ServerDataEvent(string Message) : ServerEvent;


public class TypeConverter
{
    public Type ToType(string name)
    {
        switch (name)
        {
            default:
                return typeof(int);
        }
    }

    public static TypeConverter Instance { get; } = new TypeConverter();
}

