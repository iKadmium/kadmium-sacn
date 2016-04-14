using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace kadmium_sacn
{
    public static class SACNCommon
    {
        static byte MULTICAST_BYTE_1 = (byte)239;
        static byte MULTICAST_BYTE_2 = (byte)255;
        public static int SACN_PORT = 5568;

        public static IPAddress GetMulticastAddress(Int16 universeID)
        {
            byte[] universeIDBytes = BitConverter.GetBytes(universeID).Reverse().ToArray();
            IPAddress multicastAddress = new IPAddress(new byte[] { MULTICAST_BYTE_1, MULTICAST_BYTE_2, universeIDBytes[0], universeIDBytes[1] });
            return multicastAddress;
        }
    }
}
