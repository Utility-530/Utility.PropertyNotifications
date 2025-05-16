using NetPrints.Core;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    public interface INodePin
    {
        INode Node { get; }
    }

    public interface INodeInputExecPin : INodePin, IName
    {
        IList<INodeOutputExecPin> IncomingPins { get; }
 
    }
}