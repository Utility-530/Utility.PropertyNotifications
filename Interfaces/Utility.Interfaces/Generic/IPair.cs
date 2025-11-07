namespace Utility.Interfaces.Generic
{
    public interface IPair<T>
    {
        T One { get; }

        T Two { get; }
    }
}