namespace Utility.Interfaces.NonGeneric
{
    public interface IData: IGetData, ISetData
    {


    }

    public interface IGetData
    {
        object Data { get; }
    }  
    
    public interface ISetData
    {
        object Data { set; }
    }
}
