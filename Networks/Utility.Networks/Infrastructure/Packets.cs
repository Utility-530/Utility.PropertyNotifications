using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Networks.Infrastructure
{
    public record PingPacket(Guid Guid);
    public record PongPacket(Guid Guid);
    public record MessagePacket(Guid Source, Guid? Receiver, object Value);
    public record ConnectionPacket(Guid Source, object Value);
    public record DisConnectionPacket(Guid Source);

}
