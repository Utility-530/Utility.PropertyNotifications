
// Example udp client instance
using Byter;
using Netly;
using Netly.Core;
using Utility.Infrastructure;
using Utility.Models;

internal sealed class UdpServerController : BaseObject
{
    private UdpServer? server;

    public override Key Key => new(Guids.UdpServer, nameof(UdpServerController), typeof(UdpServerController));

    public IObservable<Response> OnNext(ServerRequest serverRequest)
    {
        if (server?.Host.IPEndPoint.Address.ToString() != serverRequest.IP || server?.Host.Port != serverRequest.Port)
        {
            OnNewRequest(serverRequest);
            return Return<Response>(new ServerResponse(true));
        }
        return Return<Response>(new ServerResponse(false));

        void OnNewRequest(ServerRequest serverRequest)
        {
            server = new UdpServer();

            var serverHost = new Host(serverRequest.IP, serverRequest.Port);
            //var serverhost = new Host("127.0.0.1", 8000);

            server.OnOpen(() =>
            {
                // connection opened: server start listen client
                this.OnNext(new ServerOpenEvent());
            });

            server.OnClose(() =>
            {
                // connection closed: server stop listen client
                this.OnNext(new ServerCloseEvent());

            });

            server.OnError((exception) =>
            {
                // error on open connection
                this.OnNext(new ServerErrorEvent(exception));
            });

            server.OnEnter((client) =>
            {
                // client connected: connection accepted
                this.OnNext(new ServerEnterEvent(client));
            });

            server.OnExit((client) =>
            {
                // client disconnected: connection closed
                this.OnNext(new ServerExitEvent(client));
            });

            server.OnData((client, data) =>
            {
                // buffer/data received: {client: client instance} {data: buffer/data received} 
                //Broadcast(new ServerMessageReceivedEvent(client, new data));
                throw new Exception("898cdd w");
            });

            server.OnEvent((client, name, data) =>
            {
                Reader reader = new(data);
                var output = reader.Read<string>();
                //var type = TypeConverter.Instance.ToType(output);
                //var serialised = JsonSerializer.Deserialize(output, type);
                this.OnNext(new ClientMessageEvent(client, new ClientData(name, output)));
            });

            // open connection
            server.Open(serverHost);

        }
    }

    public IObservable<Response> OnNext(ClientMessageRequest request)
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
        server.ToEvent("chat", w.GetBytes());
        return Return(new ClientMessageResponse());
    }
}



public class Server
{
    public string IP { get; set; }
    public int Port { get; set; }
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
public record ServerDataEvent(string Message) : ServerEvent;


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

