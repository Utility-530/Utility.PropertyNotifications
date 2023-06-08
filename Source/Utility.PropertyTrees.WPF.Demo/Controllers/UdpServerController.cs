using Byter;
using Netly;
using Netly.Core;
using Utility.Infrastructure;

internal sealed class UdpServerController : BaseObject
{
    private UdpServer? server = new();

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

            server.OnOpen(() =>
            {
                // connection opened: server start listen client
                Context.Post((a) => this.Send(ServerEventsFactory.OpenEvent()), null);
            });

            server.OnClose(() =>
            {
                // connection closed: server stop listen client
                Context.Post((a) => this.Send(ServerEventsFactory.CloseEvent()), null);
            });

            server.OnError((exception) =>
            {
                Context.Post((a) => this.Send(ServerEventsFactory.ErrorEvent(exception)), null);
                // error on open connection
                //this.OnNext(new ServerErrorEvent(exception));
            });

            server.OnEnter((client) =>
            {
                // client connected: connection accepted
                this.Send(ServerEventsFactory.EnterEvent(client));
            });

            server.OnExit((client) =>
            {
                // client disconnected: connection closed
                this.Send(ServerEventsFactory.ExitEvent(client));
            });

            server.OnData((client, data) =>
            {
                // buffer/data received: {client: client instance} {data: buffer/data received} 
                Reader reader = new(data);
                var output = reader.Read<string>();
                output = reader.Read<string>();
                Context.Post((a) => this.Send(ServerEventsFactory.MessageEvent(client, new ClientData(string.Empty, output))), null);
            });

            server.OnEvent((client, name, data) =>
            {
                Reader reader = new(data);
                var output = reader.Read<string>();
                output = reader.Read<string>();
                Context.Post((a) => this.Send(ServerEventsFactory.MessageEvent(client, new ClientData(name, output))), null);
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
        server.ToEvent("chat", w.GetBytes());
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

