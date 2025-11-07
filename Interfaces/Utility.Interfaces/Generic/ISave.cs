namespace Utility.Interfaces.Generic
{
    public interface ISave<T>
    {
        bool Save(T o);
    }
}