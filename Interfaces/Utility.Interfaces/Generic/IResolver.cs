namespace Utility.Interfaces.Exs
{
    public interface IResolver<T> 
    {
        T this[string key] { get; }
    }
}
