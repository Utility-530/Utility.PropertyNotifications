using System;
using System.Collections.Generic;
using System.Text;
using Utility.Interfaces.Exs.Diagrams;

namespace Utility.Interfaces.Exs
{
    public interface IDataTracker : IObservable<ValueChange>
    {
        IObservable<INodeViewModel> Load(INodeViewModel node);
        void Track(INodeViewModel node);
    }


    public struct ValueChange
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
