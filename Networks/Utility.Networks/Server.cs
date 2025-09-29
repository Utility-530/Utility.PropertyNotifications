using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using Utility.Helpers;
using Utility.Networks.Infrastructure;

namespace Utility.Networks
{
    public record PacketEvent(string Type, Guid? Sender, Guid? Receiver, object Packet)
    {
        public DateTime Timestamp { get; } = DateTime.Now;
    }

    public record IgnoredPingInfo(DateTime LastEchoTime, TimeSpan MinimumInterval, DateTime AttemptedAt)
    {
        public override string ToString()
        {
            return $"Ping ignored at {AttemptedAt:o}, last echoed at {LastEchoTime:o}, interval required: {MinimumInterval.TotalSeconds:F1}s";
        }
    }

    public class Server : IDisposable
    {
        private readonly TcpListener _listener;
        private readonly SynchronizationContext? _context;
        private readonly ConcurrentDictionary<Guid, Client> _connections = new();
        private readonly Action<PacketEvent> onConnectionAccepted;
        private readonly Action<PacketEvent> onConnectionRemoved;
        private readonly Action<PacketEvent> onPacketReceived;
        private readonly Action<PacketEvent> onPacketSent;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _listenTask;
        private Task _monitorTask;
        private bool _disposed;

        public const string OnConnectionAccepted = nameof(OnConnectionAccepted);
        public const string OnConnectionRejected = nameof(OnConnectionRejected);
        public const string OnConnectionRemoved = nameof(OnConnectionRemoved);
        public const string OnPacketReceived = nameof(OnPacketReceived);
        public const string OnPacketSent = nameof(OnPacketSent);
        public const string OnPingIgnored = nameof(OnPingIgnored);

        // Thread-safe collections for UI binding
        public ObservableCollection<Client> Connections { get; } = new();
        public ObservableCollection<PacketEvent> PacketEvents { get; } = new();

        public IPAddress Address { get; }
        public int Port { get; }
        public bool IsRunning => _cancellationTokenSource?.Token.IsCancellationRequested == false;
        public TimeSpan PongEchoInterval { get; set; } = TimeSpan.FromSeconds(1);

        public Server(IPAddress address, 
            int port,
               Action<PacketEvent> onConnectionAccepted,
            Action<PacketEvent> onConnectionRemoved,
            Action<PacketEvent> onPacketReceived,
            Action<PacketEvent> onPacketSent
            )
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Port = port;
            this.onConnectionAccepted = onConnectionAccepted;
            this.onConnectionRemoved = onConnectionRemoved;
            this.onPacketReceived = onPacketReceived;
            this.onPacketSent = onPacketSent;
            _context = SynchronizationContext.Current;
            _listener = new TcpListener(address, port);
        }
        public static async Task<object> CreateAndStartAsync(IPAddress address, int port,
            Action<PacketEvent> onConnectionAccepted,
            Action<PacketEvent> onConnectionRemoved,
            Action<PacketEvent> onPacketReceived,
            Action<PacketEvent> onPacketSent)
        {
            var server = new Server(address, port, onConnectionAccepted, onConnectionRemoved, onPacketReceived, onPacketSent);
            await server.StartAsync();
            return server;
        }


        public async Task<bool> StartAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Server));

            if (IsRunning)
                return false;

            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _listener.Start(10);

                _listenTask = ListenForConnectionsAsync(_cancellationTokenSource.Token);
                _monitorTask = MonitorConnectionsAsync(_cancellationTokenSource.Token);

                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                await StopAsync();
                return false;
            }
        }

        public async Task StopAsync()
        {
            if (!IsRunning)
                return;

            try
            {
                _cancellationTokenSource?.Cancel();
                _listener?.Stop();

                // Wait for tasks to complete
                var tasks = new[] { _listenTask, _monitorTask }.Where(t => t != null);
                await Task.WhenAll(tasks).ConfigureAwait(false);

                // Disconnect all clients
                await DisconnectAllClientsAsync();

                _connections.Clear();
                UpdateUICollection(() => Connections.Clear());
            }
            catch (Exception ex)
            {
                // Log exception here
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task ListenForConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);

                    // Handle new connection asynchronously
                    _ = Task.Run(async () => await HandleNewConnectionAsync(tcpClient, cancellationToken), cancellationToken);
                }
                catch (ObjectDisposedException)
                {
                    // Expected when stopping
                    break;
                }
                catch (Exception ex)
                {
                    // Log exception here
                    AddEvent(OnConnectionRejected, null, null, $"Connection failed: {ex.Message}");
                }
            }
        }

        private async Task HandleNewConnectionAsync(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            Client client = null;
            try
            {
                var clientGuid = Guid.NewGuid();
                var remoteEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;

                client = new Client(clientGuid, tcpClient, remoteEndPoint);

                // Send client their GUID
                await client.SendObjectAsync(clientGuid.ToString(), cancellationToken);

                // Add to collections
                if (_connections.TryAdd(clientGuid, client))
                {
                    UpdateUICollection(() => Connections.Add(client));
                    AddEvent(OnConnectionAccepted, client, null, "Connected");

                    // Start monitoring this client
                    _ = Task.Run(async () => await MonitorClientAsync(client, cancellationToken), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                // Clean up on failure
                client?.Dispose();
                tcpClient?.Dispose();
                AddEvent(OnConnectionRejected, null, null, $"Failed to setup client: {ex.Message}");
            }
        }

        private async Task MonitorClientAsync(Client client, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && client.IsConnected)
                {
                    if (client.HasDataAvailable)
                    {
                        var receivedObject = await client.ReceiveObjectAsync<object>(cancellationToken);
                        if (receivedObject != null)
                        {
                            await ProcessReceivedObjectAsync(client, receivedObject, cancellationToken);
                        }
                    }

                    await Task.Delay(10, cancellationToken); // Small delay to prevent tight loop
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when canceling
            }
            catch (Exception ex)
            {
                // Log exception here
            }
            finally
            {
                await RemoveClientAsync(client);
            }
        }

        private async Task ProcessReceivedObjectAsync(Client sender, object receivedObject, CancellationToken cancellationToken)
        {
            try
            {
                AddEvent(OnPacketReceived, sender, null, receivedObject);

                switch (receivedObject)
                {
                    case PingPacket ping:
                        var now = DateTime.UtcNow;

                        if (now - sender.LastPongEchoed > PongEchoInterval)
                        {
                            sender.LastPongEchoed = now;
                            var pong = new PongPacket(ping.Guid);
                            await sender.SendObjectAsync(pong, cancellationToken);
                            AddEvent(OnPacketSent, sender, sender, pong);
                        }
                        else
                        {
                            var info = new IgnoredPingInfo(sender.LastPongEchoed, PongEchoInterval, now);
                            // Log ignored ping for visibility
                            AddEvent(OnPingIgnored, sender, null, info);
                        }
                        break;
                    case ServerPacket serverPacket:
                    {
                        break;
                    }
                    default:
                        await BroadcastObjectAsync(receivedObject, sender, cancellationToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log exception here
            }
        }

        private async Task BroadcastObjectAsync(object obj, Client? sender = null, CancellationToken cancellationToken = default)
        {
            var tasks = _connections.Values
                .Where(client => client != sender && client.IsConnected)
                .Select(async client =>
                {
                    try
                    {
                        await client.SendObjectAsync(obj, cancellationToken);
                        AddEvent(OnPacketSent, sender, client, obj);
                    }
                    catch (Exception ex)
                    {
                        // Log exception here - client will be removed by monitoring task
                    }
                });

            await Task.WhenAll(tasks);
        }

        private async Task MonitorConnectionsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var disconnectedClients = _connections.Values
                        .Where(client => !client.IsConnected)
                        .ToList();

                    foreach (var client in disconnectedClients)
                    {
                        await RemoveClientAsync(client);
                    }

                    await Task.Delay(1000, cancellationToken); // Check every second
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // Log exception here
                }
            }
        }

        private async Task RemoveClientAsync(Client client)
        {
            if (client == null) return;

            try
            {
                if (_connections.TryRemove(client.Guid, out _))
                {
                    UpdateUICollection(() => Connections.Remove(client));
                    AddEvent(OnConnectionRemoved, client, null, "Disconnected");
                    await BroadcastObjectAsync(new DisConnectionPacket(client.Guid));
                }

                client.Dispose();
            }
            catch (Exception ex)
            {
                // Log exception here
            }
        }

        private async Task DisconnectAllClientsAsync()
        {
            var clients = _connections.Values.ToList();
            var tasks = clients.Select(RemoveClientAsync);
            await Task.WhenAll(tasks);
        }

        private void UpdateUICollection(Action action)
        {
            if (_context != null)
            {
                _context.Post(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        private void AddEvent(string eventType, Client sender, Client receiver, object data)
        {
            try
            {
                var packetEvent = new PacketEvent(eventType, sender?.Guid, receiver?.Guid, data);
                UpdateUICollection(() =>
                {
                    switch(packetEvent.Type)
                    {
                        case OnPacketReceived :
                         onPacketReceived?.Invoke(packetEvent);
                            break;
                        case OnPacketSent:
                            onPacketSent?.Invoke(packetEvent);
                            break;
                        case OnConnectionAccepted:
                            onConnectionAccepted?.Invoke(packetEvent);
                            break;
                        case OnConnectionRemoved:
                            onConnectionRemoved?.Invoke(packetEvent);
                            break;
                        default:
                 
                            break;
                    }
                    PacketEvents.Add(packetEvent);
                });

                
            }
            catch (Exception ex)
            {
                // Log exception here
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                StopAsync().GetAwaiter().GetResult();
                _listener?.Stop();
                _disposed = true;
            }
        }

        public void SendObjectToClients(object userConnectionPacket, params Guid[] exclude)
        {
            UpdateUICollection(() =>
            {
                Connections.ForEach(async a =>
                {
                    if (exclude.Contains(a.Guid) == false)
                        await a.SendObjectAsync(userConnectionPacket);
                });
            });
        }
    }
}