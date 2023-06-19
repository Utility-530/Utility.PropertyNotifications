namespace Utility
{
    public interface IFactory<out TItem, in TArgs> 
    {
        public TItem Create(TArgs args);
    }

    public interface IFactory
    {
        public object Create(object args);
    }

}
