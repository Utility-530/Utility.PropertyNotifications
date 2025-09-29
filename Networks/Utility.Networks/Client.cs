using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using Utility.Helpers;
using Utility.Networks.Infrastructure;
using Utility.PropertyNotifications;

namespace Utility.Networks
{

    public class Client : NotifyPropertyClass, IDisposable
    {
        private readonly TcpClient _tcpClient;
        private NetworkStream _stream;
        private readonly SynchronizationContext _context;
        private bool isConnected = false;
        //private StatusType _status = StatusType.Disconnected;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _receiveTask;
        private Task _pongTask;
        private DateTime _lastPongReceived = DateTime.Now;
        private DateTime _lastPingSent = DateTime.Now;
        private bool _disposed;
        private bool raised = false;
        private TimeSpan timeSinceLastPing;
        private List<TimeSpan> delays = [];
        // Events
        private readonly Action<object>? _onDataReceived;
        private readonly Func<Client, object>? _onConnected;
        private readonly Action<Client>? _onDisconnected;

        public TimeSpan Latency => delays.Any() ? delays.Average() : TimeSpan.MinValue;
        public ObservableCollection<object> Received { get; } = new();

        public ObservableCollection<object> Sent { get; } = new();

        public Guid Guid { get; set; }

        public IPEndPoint EndPoint { get; private set; }

        public bool IsConnected
        {
            get
            {
                bool previous = isConnected;
                if (_disposed || _tcpClient?.Client == null)
                    return false;

                try
                {
                    //The TcpClient.Connected property is not reliable (it doesn’t always reflect disconnects immediately). A better test is:
                    isConnected = !(_tcpClient.Client.Poll(1, SelectMode.SelectRead) && _tcpClient.Available == 0);
                }
                catch
                {
                    return false;
                }
                if (previous != isConnected || raised == false)
                {
                    if (IsPropertyChangedNotNull)
                    {
                        raised = true;
                        RaisePropertyChanged(nameof(IsConnected));
                    }

                }
                return isConnected;
            }
        }

        public bool HasDataAvailable => _stream?.DataAvailable == true;

        public bool IsClientSide { get; }

        public int ReceiveBufferSize
        {
            get => _tcpClient?.ReceiveBufferSize ?? 0;
            set { if (_tcpClient != null) _tcpClient.ReceiveBufferSize = value; }
        }
        public DateTime LastPongEchoed { get; set; } = DateTime.MinValue;

        public int SendBufferSize
        {
            get => _tcpClient?.SendBufferSize ?? 0;
            set { if (_tcpClient != null) _tcpClient.SendBufferSize = value; }
        }

        public TimeSpan TimeSinceLastPong { get => timeSinceLastPing; set => RaisePropertyChanged(ref timeSinceLastPing, value); }

        // Constructor for server-side clients (created when server accepts connection)
        internal Client(Guid guid, TcpClient tcpClient, IPEndPoint remoteEndPoint)
        {
            Guid = guid;
            _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            EndPoint = remoteEndPoint;
            // client side connection if guid== default
            if (guid != default)
                _stream = tcpClient.GetStream();
            _context = SynchronizationContext.Current;
            _lastPongReceived = DateTime.Now;
        }

        // Constructor for client-side connections
        public Client(IPAddress address, int port,
                           Action<object>? onDataReceived = null,
                           Func<Client, object>? onConnected = null,
                           Action<Client>? onDisconnected = null) :
            this(default, new TcpClient
            {
                ReceiveBufferSize = 8192,
                SendBufferSize = 8192,
                ReceiveTimeout = 30000,
                SendTimeout = 30000
            },
            new IPEndPoint(address, port))
        {
            IsClientSide = true;
            _onDataReceived = onDataReceived;
            _onConnected = onConnected;
            _onDisconnected = onDisconnected;
        }

        public async Task<Exception?> ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Client));

            //if (Status == StatusType.Connected || Status == StatusType.Connecting)
            //    return Status == StatusType.Connected;

            try
            {
                //Status = StatusType.Connecting;
                _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                // Connect to server
                await _tcpClient.ConnectAsync(EndPoint.Address, EndPoint.Port);
                _stream = _tcpClient.GetStream();
                // Receive our GUID from server
                var guidString = await ReceiveObjectAsync<string>(_cancellationTokenSource.Token);
                if (Guid.TryParse(guidString, out var guid))
                {
                    Guid = guid;
                    //Status = StatusType.Connected;
                    _lastPongReceived = DateTime.Now;
                    _lastPingSent = DateTime.Now;

                    // Start background tasks
                    _receiveTask = MonitorIncomingDataAsync(_cancellationTokenSource.Token);
                    _pongTask = MonitorConnectionAsync(_cancellationTokenSource.Token);
                    _context.Post(async _ =>
                    {
                        var item = _onConnected?.Invoke(this);
                        await SendObjectAsync(new ConnectionPacket(Guid, item));

                    }, null);
                    return null;
                }
                else
                {
                    //Status = StatusType.Failed;
                    return new Exception($"Guid, {guidString} not valid");
                }
            }
            catch (Exception ex)
            {
                //Status = StatusType.Failed;
                // Log exception here
                return ex;
            }
        }


        public async Task DisconnectAsync()
        {
            //if (Status == StatusType.Disconnected || _disposed)
            //    return;

            try
            {
                //Status = StatusType.Disconnected;

                _cancellationTokenSource?.Cancel();

                // Wait for background tasks to complete
                //var tasks = new[] { _receiveTask, _pingTask }.Where(t => t != null);
                //await Task.WhenAll(tasks).ConfigureAwait(false);

                _stream?.Close();
                _tcpClient?.Close();
                _context?.Post(_ => _onDisconnected?.Invoke(this), null);
                _ = IsConnected;
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

        private async Task MonitorIncomingDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    var receivedObject = await ReceiveObjectAsync<object>(cancellationToken).ConfigureAwait(false);
                    if (receivedObject != null)
                        await ProcessReceivedObjectAsync(receivedObject).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
            catch
            {
                await DisconnectAsync().ConfigureAwait(false);
            }
        }


        private async Task ProcessReceivedObjectAsync(object receivedObject)
        {
            try
            {
                if (receivedObject is PongPacket pongPacket)
                {
                    _lastPongReceived = DateTime.Now;
                    delays.Add(_lastPongReceived - _lastPingSent);
                    RaisePropertyChanged(nameof(Latency));
                    i = 0;
                    // Echo ping back (for server-side clients this is handled by server)
                    if (IsClientSide == false) // This is a client-side connection
                    {
                        await SendObjectAsync(pongPacket);
                    }
                }

                // Add to events collection (UI thread safe)
                if (_context != null)
                {
                    _context.Post(_ => Received.Add(receivedObject), null);
                }
                else
                {
                    Received.Add(receivedObject);
                }

                // Notify callback
                _context.Post(_ => _onDataReceived?.Invoke(receivedObject), null);
            }
            catch (Exception ex)
            {
                // Log exception here
            }
        }



        int i = 9;
        private async Task MonitorConnectionAsync(CancellationToken cancellationToken)
        {
            const int pingIntervalMs = 1000; // 30 seconds
            const int pingTimeoutMs = 60000;  // 60 seconds

            try
            {

                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    TimeSinceLastPong = DateTime.Now - _lastPongReceived;
                    await Task.Delay(pingIntervalMs, cancellationToken);
                    // Check if we haven't received a ping response in too long


                    if (TimeSinceLastPong.TotalMilliseconds > pingTimeoutMs)
                    {
                        // Connection appears dead
                        await DisconnectAsync();
                        break;
                    }
                    i++;
                    if (i % 11 == 10)
                        // Send ping (only for client-side connections)
                        if (_onConnected != null) // This indicates it's a client-side connection
                        {

                            try
                            {
                                var pingPacket = new PingPacket(Guid);
                                await SendObjectAsync(pingPacket, cancellationToken);
                                _lastPingSent = DateTime.Now;
                            }
                            catch (Exception ex)
                            {
                                // Failed to send ping, connection probably dead
                                await DisconnectAsync();
                                break;
                            }
                        }

                }
            }
            catch (OperationCanceledException)
            {
                // Expected when canceling
            }
            catch (Exception ex)
            {
                // Log exception here
                await DisconnectAsync();
            }
        }

        public async Task<bool> SendObjectAsync<T>(T obj, CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _disposed)
                return false;

            try
            {
                Sent.Add(obj); // Add to sent collection (not UI thread safe, but should be fine for most cases)
                return await Helper.SendObjectAsync(_stream, obj, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log exception here
                await DisconnectAsync();
                return false;
            }
        }

        public async Task<T> ReceiveObjectAsync<T>(CancellationToken cancellationToken = default)
        {
            if (!IsConnected || _disposed)
                return default;

            try
            {
                return await Helper.ReceiveObjectAsync<T>(_stream, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log exception here
                await DisconnectAsync();
                return default;
            }
        }

        // Factory method for creating and connecting client
        public static async Task<object> CreateAndConnectAsync(
            string address,
            int port,
            Action<object>? onDataReceived = null,
            Func<Client, object>? onConnected = null,
            Action<Client>? onDisconnected = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (!IPAddress.TryParse(address, out var ipAddress))
                {
                    var hostEntry = await Dns.GetHostEntryAsync(address);
                    ipAddress = hostEntry.AddressList.FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork)
                              ?? hostEntry.AddressList.First();
                }

                var client = new Client(ipAddress, port, onDataReceived, onConnected, onDisconnected);

                if (await client.ConnectAsync(cancellationToken) is not Exception ex)
                {
                    return client;
                }
                else
                {
                    client.Dispose();
                    return ex;
                }
            }
            catch (Exception ex)
            {
                // Log exception here
                return ex;
            }
        }

        // Legacy static method for backward compatibility with your original API
        //public static async Task<Client> Open(string username, string address, int port,
        //    Action<object> action, Func<Client, object> connect, Action<Client> disconnect)
        //{
        //    // Note: username parameter is ignored in this implementation
        //    // You can modify this to use it if needed
        //    return await CreateAndConnectAsync(address, port, action, connect, disconnect);
        //}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    DisconnectAsync().GetAwaiter().GetResult();
                }
                catch { /* swallow exceptions during shutdown */ }

                _stream?.Dispose();
                _tcpClient?.Dispose();
                _cancellationTokenSource?.Dispose();

                _disposed = true;
            }
        }
    }
}