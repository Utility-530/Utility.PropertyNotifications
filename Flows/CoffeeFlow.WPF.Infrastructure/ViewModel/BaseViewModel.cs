using Jellyfish.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeFlow.WPF.Infrastructure.ViewModel
{

        public abstract class ObservableObject : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            protected void Set<T>(ref T field, T value, [CallerMemberName] string callerName = null)
            {
                if (!(field?.Equals(value) ?? false))
                {
                    field = value;
                    OnPropertyChanged(callerName);
                }
            }

            protected void Notify(string propertyName)
            {
                OnPropertyChanged(propertyName);
            }
        }
    
}
