using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace kadmium_sacn
{
    public class SACNSender
    {
        public Guid UUID { get; set; }
        private UdpClient Socket { get; set; }
        public IPAddress UnicastAddress { get; set; }
        public bool Multicast { get { return UnicastAddress == null; } }
        public int Port { get; set; }
        public string SourceName { get; set; }

        byte sequenceID = 0;
        
        public SACNSender(Guid uuid, string sourceName, int port)
        {
            SourceName = sourceName;
            UUID = uuid;
            Socket = new UdpClient();
            Port = port;
        }

        public SACNSender(Guid uuid, string sourceName) : this(uuid, sourceName, SACNCommon.SACN_PORT) { }

        public void Send(Int16 universeID, byte[] data)
        {
            SACNPacket packet = new SACNPacket(universeID, SourceName, UUID, sequenceID++, data);
            byte[] packetBytes = packet.ToArray();
            SACNPacket parsed = SACNPacket.Parse(packetBytes);
            Socket.Send(packetBytes, packetBytes.Length, GetEndPoint(universeID, Port));
        }

        private IPEndPoint GetEndPoint(Int16 universeID, int port)
        {
            if(Multicast)
            {
                return new IPEndPoint(SACNCommon.GetMulticastAddress(universeID), port);
            }
            else
            {
                return new IPEndPoint(UnicastAddress, port);
            }
        }
        
        public void Close()
        {
            Socket.Close();
        }
    }
}
