using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text;

namespace Utility.Networks.Infrastructure
{
    public static class Helper
    {
        private static readonly JsonSerializerSettings JsonOptions = new()
        {
            //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeNameHandling = TypeNameHandling.Objects
            //WriteIndented = false
        };

        public static async Task<bool> SendObjectAsync<T>(NetworkStream stream, T obj, CancellationToken cancellationToken = default)
        {
            if (stream == null || !stream.CanWrite || obj == null)
                return false;

            try
            {
                var json = JsonConvert.SerializeObject(obj, JsonOptions);
                var data = Encoding.UTF8.GetBytes(json);
                var lengthBytes = BitConverter.GetBytes(data.Length);

                // Send length first (4 bytes)
                await stream.WriteAsync(lengthBytes, 0, 4, cancellationToken);

                // Then send the data
                await stream.WriteAsync(data, 0, data.Length, cancellationToken);
                await stream.FlushAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public static async Task<T> ReceiveObjectAsync<T>(NetworkStream stream, CancellationToken cancellationToken = default)
        {
            if (stream == null || !stream.CanRead)
                return default;

            try
            {
                // Read length first (4 bytes)
                var lengthBytes = new byte[4];
                int bytesRead = 0;
                while (bytesRead < 4)
                {
                    int read = await stream.ReadAsync(lengthBytes, bytesRead, 4 - bytesRead, cancellationToken);
                    if (read == 0)
                        return default; // Connection closed
                    bytesRead += read;
                }

                int dataLength = BitConverter.ToInt32(lengthBytes, 0);
                if (dataLength <= 0 || dataLength > 1024 * 1024) // 1MB limit
                    return default;

                // Read the data
                var dataBytes = new byte[dataLength];
                bytesRead = 0;
                while (bytesRead < dataLength)
                {
                    int read = await stream.ReadAsync(dataBytes, bytesRead, dataLength - bytesRead, cancellationToken);
                    if (read == 0)
                        return default; // Connection closed
                    bytesRead += read;
                }

                var json = Encoding.UTF8.GetString(dataBytes);
                return JsonConvert.DeserializeObject<T>(json, JsonOptions);
            }
            catch (Exception ex)
            {
                // Log exception here
                return default;
            }
        }

        // Legacy methods for backward compatibility with your existing code
        public static T TryReceiveObject<T>(Socket socket)
        {
            if (socket == null || !socket.Connected || socket.Available == 0)
                return default;

            try
            {
                var stream = new NetworkStream(socket, false);
                return ReceiveObjectAsync<T>(stream, CancellationToken.None).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Log exception here
                return default;
            }
        }

        public static async Task<bool> SendObject<T>(this Socket socket, T obj)
        {
            if (socket == null || !socket.Connected || obj == null)
                return false;

            try
            {
                var stream = new NetworkStream(socket, false);
                return await SendObjectAsync(stream, obj, CancellationToken.None);
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public static async Task<T> ReceiveObject<T>(this Socket socket)
        {
            if (socket == null || !socket.Connected)
                return default;

            try
            {
                var stream = new NetworkStream(socket, false);
                return await ReceiveObjectAsync<T>(stream, CancellationToken.None);
            }
            catch (Exception ex)
            {
                // Log exception here
                return default;
            }
        }

        public static bool IsConnected(this Socket socket)
        {
            if (socket == null)
                return false;

            try
            {
                return socket.Connected && !socket.Poll(1000, SelectMode.SelectRead) || socket.Available > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<Guid?> ConnectTo(this Socket socket, IPEndPoint endPoint)
        {
            try
            {
                await socket.ConnectAsync(endPoint);

                // Receive GUID from server
                var guidString = await socket.ReceiveObject<string>();
                return Guid.TryParse(guidString, out var guid) ? guid : null;
            }
            catch (Exception ex)
            {
                // Log exception here
                return null;
            }
        }

        //public static async Task<bool> PingConnection(this Socket socket, Guid clientGuid)
        //{
        //    try
        //    {
        //        var pingPacket = new PingPacket(clientGuid.ToString()) ;
        //        return await socket.SendObject(pingPacket);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception here
        //        return false;
        //    }
        //}

        public static async Task<bool> SendMessage(this Socket socket, string message)
        {
            try
            {
                return await socket.SendObject(message);
            }
            catch (Exception ex)
            {
                // Log exception here
                return false;
            }
        }

        public static void Disconnect(this Socket socket)
        {
            try
            {
                if (socket?.Connected == true)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
            catch (Exception ex)
            {
                // Log exception here
                try
                {
                    socket?.Close();
                }
                catch { }
            }
        }
    }
}