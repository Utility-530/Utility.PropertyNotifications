using System.ComponentModel;
using SQLite;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;

namespace Utility.Entities
{
    public class UserProfile : Entity, IId<Guid>, IIdSet<Guid>, IComparable, ICopy, IClone
    {
        [PrimaryKey]
        [Attributes.Column(ignore: true)]
        public Guid Id { get; set; }

        [Attributes.Column(width: 120)]
        public string? Group { get; set; }

        [Attributes.Column(width: 120)]
        public string? Class { get; set; }

        [Attributes.Column(width: 120)]
        public string? Name { get; set; }

        [Attributes.Column(width: 120)]
        public string? UserName { get; set; }

        [Attributes.Column(width: 120)]
        public string? Password { get; set; }

        public string? OtherUserName { get; set; }
        public string? Pin { get; set; }
        public string? URL { get; set; }
        public string? SecretQuestion { get; set; }
        public string? MemorableWord { get; set; }

        [ReadOnly(true)]
        public DateTime AddDate { get; set; }

        public int CompareTo(object? obj)
        {
            if (obj is UserProfile m)
            {
                return m.AddDate.CompareTo(m.AddDate);
            }
            return 0;
        }

        public object Clone()
        {
            var clone = new UserProfile
            {
                Id = Guid.NewGuid(),
                Group = Group,
                Class = Class,
                Name = Name,
                UserName = UserName,
                Password = Password,
                OtherUserName = OtherUserName,
                Pin = Pin,
                URL = URL,
                SecretQuestion = SecretQuestion,
                MemorableWord = MemorableWord
            };
            return clone;
        }

        public string Copy() => Password;
    }
}