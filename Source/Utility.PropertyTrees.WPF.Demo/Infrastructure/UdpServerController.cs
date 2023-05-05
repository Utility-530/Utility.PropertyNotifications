
// Example udp client instance
using Byter;
using Netly;
using Netly.Core;
using System;
using System.Text;
using System.Text.Json;
using Utility.Infrastructure;
using Utility.Models;

// Example host instance
//var clienthost = new Host("127.0.0.1", 8000);
namespace Utility.PropertyTrees.WPF.Demo;

public class Server
{
    public string IP { get; set; }
    public int Port { get; set; }
}

public class UdpServerController : BaseObject
{
    private UdpServer? server;
    readonly Guid guid = Guid.Parse("2978ac5e-368f-42b3-891e-d85c21c14499");

    public override Key Key => new(guid, nameof(UdpServerController), typeof(UdpServerController));

    public override void OnNext(object value)
    {
        if (value is ServerRequest { IP: var ip, Port: var port } serverRequest)
        {
            if (server?.Host.IPEndPoint.Address.ToString() != ip || server.Host.Port != port)
            {
                OnNewRequest(serverRequest);
            }
        }
        else if (value is ClientMessageRequest request)
        {
            OnNextMessage(request);
        }
        else
        {
            base.OnNext(value);
        }

        void OnNextMessage(ClientMessageRequest clientMessageRequest)
        {
            if (server is null)
            {
                throw new Exception("sdfg 090900");
            }
            //Check for exit conditions
            //line = Console.ReadLine();
            using Writer w = new();
            //w.Write("8000");
            //w.Write(false);
            //w.Write("*");
            w.Write(clientMessageRequest.Message);
            server.ToEvent("chat", w.GetBytes());
        }

        void OnNewRequest(ServerRequest serverRequest)
        {
            server = new UdpServer();

            var serverHost = new Host(serverRequest.IP, serverRequest.Port);
            //var serverhost = new Host("127.0.0.1", 8000);

            server.OnOpen(() =>
            {
                // connection opened: server start listen client
                Broadcast(new ServerOpenEvent());
            });

            server.OnClose(() =>
            {
                // connection closed: server stop listen client
                Broadcast(new ServerCloseEvent());

            });

            server.OnError((exception) =>
            {
                // error on open connection
                Broadcast(new ServerErrorEvent(exception));
            });

            server.OnEnter((client) =>
            {
                // client connected: connection accepted
                Broadcast(new ServerEnterEvent(client));
            });

            server.OnExit((client) =>
            {
                // client disconnected: connection closed
                Broadcast(new ServerExitEvent(client));
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
                Broadcast(new ClientResponseEvent(client, new ClientData(name, output)));
            });

            // open connection
            server.Open(serverHost);

        }
    }
    public record ServerRequest(string IP, int Port);

    public record ClientMessageRequest(string Message);
    public record ServerEvent();
    public record ServerOpenEvent() : ServerEvent;
    public record ServerCloseEvent() : ServerEvent;
    public record ServerErrorEvent(Exception Exception) : ServerEvent;
    public record ServerEnterEvent(UdpClient Client) : ServerEvent;
    public record ServerExitEvent(UdpClient Client) : ServerEvent;
    public record ClientResponseEvent(UdpClient Client, ClientData ClientData) : ServerEvent;

    public record ClientData(string Header, object? Message);
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
}
