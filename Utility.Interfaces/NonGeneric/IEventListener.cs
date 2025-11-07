namespace Utility.Interfaces.NonGeneric
{
    public interface IEventListener
    {
        public void Send(IEvent @event);
    }
}