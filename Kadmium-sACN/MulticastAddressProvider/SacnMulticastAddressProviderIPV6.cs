using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kadmium_sACN.MulticastAddressProvider
{
	public class SacnMulticastAddressProviderIPV6 : ISacnMulticastAddressProvider
	{
		public IPAddress GetMulticastAddress(UInt16 universe)
		{
			var universeBytes = GetUniverseBytes(universe);
			string ipString = $"FF18::83:00:{universeBytes[0].ToString("X2")}:{universeBytes[1].ToString("X2")}";
			IPAddress address = IPAddress.Parse(ipString);
			return address;
		}

		private static Span<byte> GetUniverseBytes(UInt16 universe)
		{
			byte[] universeBytes = new byte[2];
			BinaryPrimitives.WriteUInt16BigEndian(universeBytes, universe);
			return universeBytes;
		}
	}
}
