namespace Utility.Interfaces.NonGeneric
{
    public interface IFactory
    {
        object Create(object config);
    }
}