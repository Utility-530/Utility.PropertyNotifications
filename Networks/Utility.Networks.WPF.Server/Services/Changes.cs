using Simple.Models;

namespace Utility.Networks.WPF.Server.Services
{
    public readonly record struct UserNameAddChange(string Key, object Value) : IDictionaryAddChange
    {
    }
    public readonly record struct UserNameRemoveChange(string Key) : IDictionaryRemoveChange
    {
    }

    public readonly record struct StatusChange(string Value) : IStringChange
    {
    }
    public readonly record struct IsRunningChange(bool Value) : IBooleanChange
    {
    }
    public readonly record struct ExternalAddressChange(string Value) : IStringChange
    {
    }


    public readonly record struct PortChange(string Value) : IStringChange
    {
    }
    public readonly record struct ServerChange(object Value) : IObjectChange
    {
    }

    public readonly record struct ClientsConnectedChange(int Value) : IIntChange
    {
    }

    public readonly record struct RunChange() : IChange
    {
        public DateTime TimeStamp { get; } = DateTime.Now;
    }

    public readonly record struct StopChange() : IChange
    {
        public DateTime TimeStamp { get; } = DateTime.Now;
    }
}
