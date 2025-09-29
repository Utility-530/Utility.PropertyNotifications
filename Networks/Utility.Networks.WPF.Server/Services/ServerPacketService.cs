using Utility.Networks.Infrastructure;

namespace Utility.Networks.WPF.Server.Services
{
    internal class ServerPacketService : Utility.Interfaces.Generic.IProcess<ServerPacket>
    {
        public ServerPacketService()
        {
        }

        public void Process(ServerPacket packet)
        {
            if (packet is { Value:{ } value  })
                switch (value)
                {

                    default:
                        Console.WriteLine("Unknown packet type received.");
                        break;
                }
        }
    }
}
