using Simple.Models;

namespace Utility.Networks.WPF.Server.Services
{
    public readonly record struct ConnectParameters(string Address,
            int Port,
            Action<object>? OnDataReceived = null,
            Func<object, object>? OnConnected = null,
            Action<object>? OnDisconnected = null) 
    {
    }

    public readonly record struct ConnectChange(object Value) : IObjectChange
    {
    }

    public readonly record struct SendChange(string Value) : IStringChange
    {
        public DateTime TimeStamp { get; } = DateTime.Now;
    }

    public readonly record struct DisconnectChange() : IChange
    {
        public DateTime TimeStamp { get; } = DateTime.Now;
    }
}
