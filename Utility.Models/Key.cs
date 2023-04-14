//using Newtonsoft.Json;
using System.Text.Json;
using Utility.Randoms;

namespace Utility.Models
{
    public class Key : ISerialise, IEquatable<Key>, IEquatable<ISerialise>
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }

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

        public static Key Random()
        {
            return new Key
            {
                Guid = Guid.NewGuid(),
                Name = Names.Values[(int)(DateTime.Now.Ticks % Names.Values.Count)]
            };
        }

        public bool Equals(Key? other)
        {
            return Name == other?.Name && Guid == other?.Guid;
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
}