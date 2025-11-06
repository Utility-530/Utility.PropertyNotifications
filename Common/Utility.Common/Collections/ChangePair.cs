using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Utility.Common.Models
{
    /// <summary>
    /// From <a href="https://www.broculos.net/2014/03/wpf-editable-datagrid-and.html"></a>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ChangePair<TKey, TValue> : INotifyPropertyChanged
    {
        protected TKey key;
        protected TValue value;

        public ChangePair()
        {
        }

        public ChangePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public ChangePair(KeyValuePair<TKey, TValue> kv)
        {
            Key = kv.Key;
            Value = kv.Value;
        }

        public TKey Key
        {
            get => key;
            set => OnPropertyChanged(ref key, value);
        }

        public TValue Value
        {
            get => value;
            set => OnPropertyChanged(ref this.value, value);
        }

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Occurs when any properties are changed on this object.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///  raises the PropertyChanged event for a single property
        ///  'propertyname' can be left null (e.g OnPropertyChanged()), if called from body of property
        /// </summary>
        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string? propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Implementation
    }
}