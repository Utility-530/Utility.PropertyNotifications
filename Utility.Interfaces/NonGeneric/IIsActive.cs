namespace Utility.Interfaces.NonGeneric
{
    public interface IIsActive: IGetIsActive, ISetIsActive
    {
    }
    public interface IGetIsActive
    {
        bool IsActive { get; }

    }

    public interface ISetIsActive
    {
        bool IsActive { set; }
    }
}
