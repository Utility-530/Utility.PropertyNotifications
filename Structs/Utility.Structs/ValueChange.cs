
namespace Utility.Structs
{
    public readonly struct ValueChange
    {
        public ValueChange(string name, object value) : this()
        {
            Name = name;
            Value = value;
        }

        public string Name { get;  }

        public object Value { get;  }
    }
}
