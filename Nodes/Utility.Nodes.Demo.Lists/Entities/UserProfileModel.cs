using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Attributes;
using Utility.Interfaces.Generic.Data;
using Utility.PropertyNotifications;

namespace Utility.Nodes.Demo.Lists.Entities
{
    [Model]
    public class UserProfileModel : NotifyPropertyClass, IId<Guid>
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string? Group { get; set; }
        public string? Type { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? OtherUserName { get; set; }
        public string? Pin { get; set; }
        public string? URL { get; set; }
        public string? SecretQuestion { get; set; }
        public string? MemorableWord { get; set; }
        public DateTime AddDate { get; set; }

    }
}

