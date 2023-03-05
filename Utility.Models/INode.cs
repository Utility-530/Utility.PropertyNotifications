using System.Reactive.Subjects;
using Utility.Interfaces.Generic;
using Utility.Interfaces.Generic.Data;

namespace Utility.Models
{
    public interface INode : IKey<Key>, ISubject<Message> { }
}
