using NetPrints.Interfaces;

namespace NetPrints.Graph
{
    public interface IDataPin : IName
    {
        IBaseType PinType { get; set; }
    }
}