namespace Utility.WPF.Meta
{
    public record KeyValue(string Key)
    {      
        public virtual object? Value { get; }
        public virtual string GroupKey { get; }
    }

}