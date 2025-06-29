namespace Utility.Enums
{
    public interface ITitle
    {
        string Title { get; set; }
    }


    public interface IGetTitle
    {
        string Title { get; }
    }

    public interface ISetTitle
    {
        string Title { set; }
    }
}
