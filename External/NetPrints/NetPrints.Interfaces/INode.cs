using NetPrints.Core;
using NetPrints.Graph;
using System;
using System.Collections.Generic;

namespace NetPrints.Interfaces
{
    public interface INode: IName
    {
        INodeGraph Graph { get; }
        IObservableCollection<INodeOutputExecPin> OutputExecPins { get; }
        IObservableCollection<INodeInputExecPin> InputExecPins { get; }
        IObservableCollection<INodeInputDataPin> InputDataPins { get; }
        IObservableCollection<INodeInputTypePin> InputTypePins { get; }
        IObservableCollection<INodeOutputTypePin> OutputTypePins { get; }
        IObservableCollection<INodeOutputDataPin> OutputDataPins { get; }
        bool IsPure { get; }
        double PositionX { get; set; }
        double PositionY { get; set; }

        event EventHandler InputTypeChanged;

        void OnMethodDeserialized();
    }
}
