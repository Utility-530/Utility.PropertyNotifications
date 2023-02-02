using System;

namespace UtilityWpf.Demo.Common.ViewModels
{
    public class Fields : IEquatable<Fields>
    {
        private string? name;
        private string? surname;
        private int age;
        private string? phoneNumber;

        public Guid Id { get; init; }
        public string Name { get => name ?? ""; set => name = value; }
        public string Surname { get => surname ?? ""; set => surname = value; }
        public int Age { get => age; set => age = value; }
        public string PhoneNumber { get => phoneNumber ?? ""; set => phoneNumber = value; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Fields);
        }

        public bool Equals(Fields? obj)
        {
            return $"{Id} {Name} {Surname} {Age} {PhoneNumber}".Equals(obj != null ? $"{obj.Id} {obj.Name} {obj.Surname} {obj.Age} {obj.PhoneNumber}" : "");
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Id.ToByteArray(), 0);
        }

        public override string ToString()
        {
            return $"{Id}";
            //  return $"{Id} {Name} {Surname} {Age} {PhoneNumber}";
        }
    }
}