using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Reactives;

namespace Utility.Models.Diagrams
{
    public class MethodConnector : ReactiveProperty<object>, IKey, IType
    {
        public string Key { get; set; }

        public MethodConnector() : base(default)
        {
            this.WithChangesTo(a => a.Value)
                .Subscribe(a =>
                {
                    IsActive = true;
                    OnPropertyChanged(nameof(IsActive));
                });
        
        }

        public bool IsActive { get; set; }

        public Type Type { get; set; }
    }

}
