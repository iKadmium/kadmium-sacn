using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using kadmium_sacn;
using System.Threading;
using EventSocket;

namespace kadmium_sacn
{
    public class SACNListener
    {
        EventClient Socket { get; set; }
        Int16 UniverseID { get; set; }
        private bool DataAvailableFlagged { get; set; }
        public event EventHandler<PacketReceivedEventArgs> OnReceive;
        public int Port { get; set; }

        public SACNListener(Int16 universeID, int port)
        {
            Port = port;
            UniverseID = universeID;
            Socket = new EventClient(port);
            Socket.JoinMulticastGroup(SACNCommon.GetMulticastAddress(UniverseID));

            Socket.PacketReceived += Socket_PacketReceived;
        }

        private void Socket_PacketReceived(object sender, PacketReceivedArgs e)
        {
            SACNPacket packet = SACNPacket.Parse(e.Packet);
            OnReceive?.Invoke(this, new PacketReceivedEventArgs(packet));
        }

        public SACNListener(Int16 universeID) : this(universeID, SACNCommon.SACN_PORT) { }
        
        public class PacketReceivedEventArgs : EventArgs
        {
            public SACNPacket Packet { get; set; }
            public PacketReceivedEventArgs(SACNPacket packet)
            {
                Packet = packet;
            }
        }
    }
}
