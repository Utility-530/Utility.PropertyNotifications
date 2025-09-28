using Simple.Models;
using System.Net;
using System.Windows;
using Utility.Networks;
using Utility.Networks.Infrastructure;
using Utility.Networks.Packets;
using Utility.Networks.WPF.Server.Services;

namespace ChatterServer.Services
{
    internal class Service
    {
        private Server _server => (Server)Model.Instance.Object<ServerChange>();
        private Task _updateTask;
        private Task _listenTask;

        void onNext<T>(T change) where T : IChange => Model.Instance.AddChange(change);

        Dictionary<Guid, UserDetails> Users = new();

        public Service()
        {
            onNext(new PortChange("8000"));
            onNext(new StatusChange("Idle"));

            Model.Instance.Changes.CollectionChanged += async (s, e) =>
            {
                foreach (IChange item in e.NewItems)
                {
                    switch (item)
                    {
                        case RunChange outputChange:
                            Run();
                            break;
                        case StopChange outputChange:
                            await Stop();
                            break;
                    }
                }
            };

            void Run()
            {
                onNext(new StatusChange("Connecting..."));
                setup();
                _listenTask = Task.Run(() => _server.StartAsync());
                onNext(new IsRunningChange(true));
                update();


                void setup()
                {
                    onNext(new StatusChange("Validating socket..."));

                    if (!int.TryParse(Model.Instance.String<PortChange>(), out var socketPort))
                    {
                        DisplayError("Port value is not valid!");
                        return;
                    }

                    //await Task.Run(() => onNext(new ExternalAddressChange(IPHelper.InternalIp())));
                    onNext(new ServerChange(new Server(IPAddress.Any, socketPort)));

                    onNext(new StatusChange("Server set up"));
                    _server.PacketEvents.CollectionChanged += (s, e) =>
                    {
                        foreach (PacketEvent item in e.NewItems)
                        {
                            switch (item.Type)
                            {
                                case Server.OnConnectionAccepted:
                                    Server_OnConnectionAccepted(item.Sender, item);
                                    break;
                                case Server.OnConnectionRemoved:
                                    Server_OnConnectionRemoved(item.Sender, item);
                                    break;
                                case Server.OnPacketReceived:
                                    Server_OnPacketReceived(item.Sender, item);
                                    break;
                                case Server.OnPacketSent:
                                    Server_OnPacketSent(item.Sender, item);
                                    break;
                            }
                        }
                    };


                    void Server_OnPacketSent(object sender, PacketEvent e)
                    {
                    }

                    void Server_OnPacketReceived(object sender, PacketEvent e)
                    {
                        if (e.Packet is ConnectionPacket { Value: UserDetails userDetails } ucp)
                        {
                            Users.Add(ucp.Source, userDetails);
                        }
                        if (e.Packet is DisConnectionPacket dis)
                        {
                            Users.Remove(dis.Source);
                        }
                    }

                    void Server_OnConnectionAccepted(object sender, PacketEvent e)
                    {               
                    }

                    void Server_OnConnectionRemoved(object sender, PacketEvent e)
                    {
                        if (e.Sender is Guid guid)
                        {
                            //Users.Remove(guid);
                            Task.Run(() => _server.SendObjectToClients(new DisConnectionPacket(guid)));
                        }
                    }
                }

                async Task update()
                {
                    while (Model.Instance.Bool<IsRunningChange>())
                    {
                        await Task.Delay(5);
                        if (!_server.IsRunning)
                        {
                            await Stop();
                            return;
                        }
                        onNext(new StatusChange("Running"));
                    }
                }
            }
        }

        private async Task Stop()
        {
            onNext(new IsRunningChange(false));
            _server.StopAsync();
            await _listenTask;
            //await _updateTask;
            onNext(new StatusChange("Stopped"));
        }

        void DisplayError(string message)
        {
            MessageBox.Show(message, "Woah there!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
