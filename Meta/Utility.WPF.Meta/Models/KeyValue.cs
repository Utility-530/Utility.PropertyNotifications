namespace Utility.WPF.Meta
{
    public class KeyValue
    {
        public KeyValue(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public KeyValue(string key)
        {
            Key = key;
        }

        public string Key { get; }

        public virtual object? Value { get; }

        public virtual string GroupKey { get; }
    }

}