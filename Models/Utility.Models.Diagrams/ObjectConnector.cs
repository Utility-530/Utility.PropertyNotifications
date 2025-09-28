using System.ComponentModel;
using System.Reflection;
using Utility.Interfaces.NonGeneric;
using Utility.PropertyNotifications;
using Utility.Reactives;

namespace Utility.Models.Diagrams
{
    public class ObjectConnector : ReactiveProperty<object>, IKey
    {
        public string Key { get; set; }

        public ObjectConnector(INotifyPropertyChanged instance, PropertyInfo propertyInfo) : base(default)
        {
            instance.WithChangesTo(propertyInfo)
                .Subscribe(a =>
                {
                    IsActive = true;
                    OnPropertyChanged(nameof(IsActive));
                });
            Property = propertyInfo;
        
        }

        public bool IsActive { get; set; }

        public PropertyInfo Property { get; set; }
    }

}
