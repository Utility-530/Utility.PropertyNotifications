using System;

namespace NetPrints.Interfaces
{
    public interface INodeOutputExecPin : IName
    {
        INodeInputExecPin OutgoingPin { get; set; }
        event OutputExecPinOutgoingPinChangedDelegate OutgoingPinChanged;
    }

    public delegate void OutputExecPinOutgoingPinChangedDelegate(
    INodeOutputExecPin pin, INodeInputExecPin oldPin, INodeInputExecPin newPin);

}