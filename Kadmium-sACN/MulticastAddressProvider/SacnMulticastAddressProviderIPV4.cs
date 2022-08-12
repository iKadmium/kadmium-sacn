using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Kadmium_sACN.MulticastAddressProvider
{
	public class SacnMulticastAddressProviderIPV4 : ISacnMulticastAddressProvider
	{
		public IPAddress GetMulticastAddress(UInt16 universe)
		{
			var universeBytes = GetUniverseBytes(universe);
			IPAddress address = new IPAddress(new byte[] { 239, 255, universeBytes[0], universeBytes[1] });
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
