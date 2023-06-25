using System.Formats.Asn1;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utility.Interfaces.NonGeneric;

namespace Utility.Models
{

    public class Key<T> : Key
    {
        public Key(Guid guid) : base(guid, typeof(T).Name, typeof(T))
        {
        }
    }

    public class StringKey : IEquatable
    {
        public StringKey(string? value)
        {
            Value = value;
        }

        public bool Equals(IEquatable? other)
        {
            return (other as StringKey)?.Value.Equals(Value) == true;
        }

        public string? Value { get; init; }
    }

    public class Key : ISerialise, IGuid, IType, IName, IEquatable, IEquatable<ISerialise>
    {
        public Key(Guid guid, string name, Type type)
        {
            Guid = guid;
            Name = name;
            Type = type;
        }

        public Key()
        {
        }

        public Guid Guid { get; set; }
        public string Name { get; set; }

        [JsonConverter(typeof(JsonTypeConverter))]
        public Type Type { get; set; }

        public Key Child { get; set; }

        public ISerialise FromString(string str)
        {
            var obj = JsonSerializer.Deserialize<Key>(str);
            Guid = obj.Guid;
            Name = obj.Name;
            return this;
        }

        public override string ToString()
        {
            var obj = JsonSerializer.Serialize(this);
            return obj;
        }


        public bool Equals(IEquatable? other)
        {
            return Equals(other as Key);
        }

        public bool Equals(Key? other)
        {
            return Name.Equals(other?.Name) && Guid.Equals(other?.Guid);// && Type.Equals(other?.Type);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Key);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool Equals(ISerialise? other)
        {
            return Equals(other as Key);
        }
    }

    public class JsonTypeConverter : JsonConverter<Type>
    {
        public override Type Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
               Type.GetType(reader.GetString());

        public override void Write(
            Utf8JsonWriter writer,
            Type dateTimeValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue($"{dateTimeValue.Namespace}.{dateTimeValue.Name}, {dateTimeValue.AssemblyQualifiedName}");
    }
}