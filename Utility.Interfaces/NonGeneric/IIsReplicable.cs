namespace Utility.Interfaces.NonGeneric
{
    public interface IIsReplicable
    {
        bool IsReplicable { get; set; }
    }   
    
    public interface IGetIsReplicable
    {
        bool IsReplicable { get; }
    } 
    
    public interface ISetIsReplicable
    {
        bool IsSetReplicable { get; }
    }
}
