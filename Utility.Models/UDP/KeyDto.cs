using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Models.UDP
{
    public record KeyDto(Guid Guid, string Name);

    public record GuidDto(Guid Source);

    public record TypeDto(string InName, string OutName);

}
