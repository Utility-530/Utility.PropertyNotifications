namespace Utility.Interfaces.NonGeneric
{
    public interface IData
    {
        object Data { get; set; }

    }

    public interface IGetData
    {
        object Data { get; }
    }  
    
    public interface ISetData
    {
        object Data { get; }
    }
}
