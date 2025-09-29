using Simple.Models;
using Splat;
using System.Net;
using System.Windows;
using Utility.Interfaces.Generic;
using Utility.Networks;
using Utility.Networks.Infrastructure;
using Utility.Networks.Packets;
using Utility.Networks.WPF.Server.Services;
using Utility.PropertyNotifications;

namespace ChatterServer.Services
{
    internal class ServerService : NotifyPropertyClass
    {
        private Server _server;

        void onNext<T>(T change) where T : IChange => Model.Instance.AddChange(change);

        Dictionary<Guid, UserDetails> users = new();

        public ServerService()
        {
            Model.Instance.Changes.CollectionChanged += async (s, e) =>
            {
                foreach (IChange item in e.NewItems)
                {
                    switch (item)
                    {
                        case RunChange outputChange:
                            onNext(new StatusChange("Validating socket..."));

                            if (!int.TryParse(Model.Instance.String<PortChange>(), out var socketPort))
                            {
                                displayError("Port value is not valid!");
                                break;
                            }

                            var x = await Utility.Networks.Server.CreateAndStartAsync(
                                IPAddress.Any,
                                socketPort,
                                a =>
                                {
                                },
                                e =>
                                {
                                    if (e.Sender is Guid guid)
                                    {
                                        Task.Run(() => _server.SendObjectToClients(new DisConnectionPacket(guid)));
                                    }
                                },
                                e =>
                                {
                                    if (e.Packet is ConnectionPacket { Value: UserDetails userDetails } ucp)
                                    {
                                        users.Add(ucp.Source, userDetails);
                                    }
                                    if (e.Packet is DisConnectionPacket dis)
                                    {
                                        users.Remove(dis.Source);
                                    }
                                    if (e.Packet is ServerPacket serverPacket)
                                    {
                                        Locator.Current.GetService<IProcess<ServerPacket>>().Process(serverPacket);
                                    }
                                },
                                a =>
                                {
                                });

                            if (x is Exception ex)
                            {
                                displayError(ex.Message);
                                onNext(new StatusChange("Error"));
                                break;
                            }
                            else if (x is Server server)
                            {
                                _server = server;
                            }

                            update();
                            RaisePropertyChanged(nameof(Server));
                            onNext(new IsRunningChange(true));
                            onNext(new StatusChange("Server set up"));
                            break;
                        case StopChange outputChange:
                            {
                                onNext(new IsRunningChange(false));
                                await _server.StopAsync();
                                onNext(new StatusChange("Stopped"));
                            }
                            break;
                    }
                }

                async Task update()
                {
                    while (Model.Instance.Bool<IsRunningChange>())
                    {
                        await Task.Delay(5);
                        if (!_server.IsRunning)
                        {
                            onNext(new StopChange());
                            return;
                        }
                        onNext(new StatusChange("Running"));
                    }
                }
            };
        }

        public Server Server => _server;

        void displayError(string message)
        {
            MessageBox.Show(message, "Woah there!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
