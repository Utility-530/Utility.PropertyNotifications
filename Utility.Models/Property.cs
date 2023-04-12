using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Utility.Models
{
    public class Property : INotifyPropertyChanged
    {
        private object _value;

        public int GridRow { get; set; }
        public int GridColumn { get; set; }
        public int GridColumnSpan { get; set; }
        public int GridRowSpan { get; set; }



        public Property(object? value = null) => _value = value;

        public object Value
        {
            get => _value;
            set
            {
                if (_value?.Equals(value) != true)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        #region propertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

    public class Property<T> : Property
    {
        public Property(T value) => Value = value;
        public Property() { }

        public T GetValue() => (T)Value;

        public void SetValue(T value) => Value = value;
    }
}
