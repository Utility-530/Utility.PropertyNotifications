using System;
using System.Collections;
using System.ComponentModel;
using UtilityInterface.Generic;

namespace ReactiveAsyncWorker.ViewModel
{
    public class KeyValue : IKey<string>, IComparable
    {
        public KeyValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }

        public int CompareTo(object obj)
        {
            return obj is IKey<string> key ? this.Key.CompareTo(key.Key) : 0;
        }
    }

    public class KeyCollection : IKey<string>, IComparable, IComparable<KeyCollection>
    {
        public KeyCollection(string key, IEnumerable value)
        {
            Key = key;
            Collection = value;
        }

        public string Key { get; }

        public IEnumerable Collection { get; }

        public int CompareTo(object obj)
        {
            return obj is IKey<string> key ? this.Key.CompareTo(key.Key) : 0;
        }

        public int CompareTo(KeyCollection other)
        {
            return this.Key.CompareTo(other.Key);
        }
    }

    public class KeyRange : INotifyPropertyChanged, IKey<string>, IComparable
    {
        private double value;

        public KeyRange(string key, double value, double? min = null, double? max = null)
        {
            Key = key;
            Value = value;
            Min = min ?? 0;
            Max = max ?? value * 2;
        }

        public int TickFrequency => (int)((Max - Min) / 10);
        public string Key { get; set; }
        public double Value { get { return value; } set { this.value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); } }
        public double Min { get; set; }
        public double Max { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        public int CompareTo(object obj)
        {
            return obj is IKey<string> key ? this.Key.CompareTo(key.Key) : 0;
        }
    }
}