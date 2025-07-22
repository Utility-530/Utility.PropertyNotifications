using System.ComponentModel;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{
    public interface IValueModel : IValue, ISetValue, ISet, INotifyPropertyCalled, INotifyPropertyReceived, INotifyPropertyChanged
    {

    }
}
