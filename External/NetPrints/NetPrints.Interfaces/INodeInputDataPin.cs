using NetPrints.Core;
using NetPrints.Interfaces;
using System;
using System.ComponentModel;

namespace NetPrints.Graph
{
    public interface INodeInputDataPin : IDataPin, INodePin
    {
        INodeOutputDataPin IncomingPin { get; set; }
        event InputDataPinIncomingPinChangedDelegate IncomingPinChanged;
        bool UsesExplicitDefaultValue { get; set; }
        object ExplicitDefaultValue { get; set; }
        object UnconnectedValue { get; set; }
        bool UsesUnconnectedValue { get; }
    }

    public delegate void InputDataPinIncomingPinChangedDelegate(
    INodeInputDataPin pin, INodeOutputDataPin oldPin, INodeOutputDataPin newPin);

    public interface INodeTypePin: IInferredType, IName, INotifyPropertyChanged
    {

    }

    public interface INodeInputTypePin: INodeTypePin, IInferredType, IName
    {
        INodeOutputTypePin IncomingPin { get; set; }
        event InputTypePinIncomingPinChangedDelegate IncomingPinChanged;
    }

    public delegate void InputTypePinIncomingPinChangedDelegate(
    INodeInputTypePin pin, INodeOutputTypePin oldPin, INodeOutputTypePin newPin);


    public interface INodeOutputTypePin: INodeTypePin, INodePin
    {
        IObservableCollection<INodeInputTypePin> OutgoingPins { get; }
    }

    public interface IInferredType
    {
        IBaseType InferredType { get; set; }

    }
}