using SQLite;
using System.ComponentModel;
using Utility.Attributes;
using Utility.Interfaces.Generic.Data;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.Demo.Lists.Factories;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Entities
{
    [Model("d17c5de2-7836-4c02-958c-eb1de974f474", nameof(NodeMethodFactory.BuildUserProfileRoot), 1)]
    public class UserProfileModel : NotifyPropertyClass, IId<Guid>, IIdSet<Guid>, IComparable, ICopy, IClone
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

        [Attributes.Column(width:120)]
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
            if(obj is UserProfileModel m)
            {
                return m.AddDate.CompareTo(m.AddDate);
            }
            return 0;
        }

        public object Clone()
        {
            var clone = AnyClone.CloneExtensions.Clone(this);
            clone.Id = Guid.NewGuid();
            clone.Group = this.Group;
            clone.Class = this.Class;
            clone.Name = this.Name;
            clone.UserName = this.UserName;
            clone.Password = this.Password;
            clone.OtherUserName = this.OtherUserName;
            clone.Pin = this.Pin;
            clone.URL = this.URL;
            clone.SecretQuestion = this.SecretQuestion;
            clone.MemorableWord = this.MemorableWord;
            return clone;
        }


        public string Copy() => Password;
    }
}

