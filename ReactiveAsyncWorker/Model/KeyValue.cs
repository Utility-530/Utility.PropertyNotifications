using System.Collections;
using System.ComponentModel;

namespace ReactiveAsyncWorker.ViewModel
{
    public class KeyValue
    {
        public KeyValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public object Value { get; }
    }

    public class KeyCollection
    {
        public KeyCollection(string key, IEnumerable value)
        {
            Key = key;
            Collection = value;
        }

        public string Key { get; }

        public IEnumerable Collection { get; }
    }

    public class KeyRange : INotifyPropertyChanged
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
    }
}