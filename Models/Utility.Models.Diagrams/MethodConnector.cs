using System.Reflection;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Reactives;

namespace Utility.Models.Diagrams
{
    public class MethodConnector : ReactiveProperty<object>, IKey, IParameterInfo, IDisposable
    {
        private IDisposable disposable { get; }
        public string Key { get; set; }

        public MethodConnector() : base(default)
        {
            disposable = this.WithChangesTo(a => a.Value)
                .Subscribe(a =>
                {
                    IsActive = true;
                    OnPropertyChanged(nameof(IsActive));
                });

        }

        public bool IsActive { get; set; }

        public ParameterInfo Parameter { get; set; }


        public void Dispose()
        {
             disposable.Dispose();
        }
    }
}