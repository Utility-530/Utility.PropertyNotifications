using Simple.Models;
using Utility.Networks.Infrastructure;
using Utility.Networks.WPF.Server.Services;
using Utility.PropertyNotifications;

namespace Utility.Networks.WPF.Client.Services
{
    internal class ClientService : NotifyPropertyClass
    {
        private Utility.Networks.Client client;

        public ClientService()
        {
            Model.Instance.Changes.CollectionChanged += async (s, e) =>
            {
                foreach (IChange item in e.NewItems)
                {
                    switch (item)
                    {
                        case ConnectChange
                        {
                            Value:
                            ConnectParameters
                            {
                                Address: { } address,
                                Port: { } port,
                                OnConnected: { } onConnected,
                                OnDataReceived: { } onDataReceived,
                                OnDisconnected: { } onDisconnected
                            }
                        }:
                            switch (await Networks.Client.CreateAndConnectAsync(address, port, onDataReceived, onConnected, onDisconnected))
                            {
                                case Exception ex:
                                    Model.Instance.AddChange(new ExceptionChange(ex));
                                    break;
                                case Utility.Networks.Client c:
                                    client = c;
                                    RaisePropertyChanged(nameof(Client));
                                    break;
                            }
                            break;
                        case DisconnectChange outputChange:
                            await client.DisconnectAsync();
                            break;
                        case SendChange { Value: { } value } sendChange:
                            await client.SendObjectAsync(new MessagePacket(client.Guid, null, value));
                            break;
                    }
                }
            };

        }

        public Networks.Client Client => client;
    }
}
