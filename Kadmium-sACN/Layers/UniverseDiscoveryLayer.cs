using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public class UniverseDiscoveryLayer : SACNLayer
	{
		public const UInt32 VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST = 0x00000001;
		public const int MIN_LENGTH = 8;

		public UInt32 Vector { get; set; }
		public byte Page { get; set; }
		public byte LastPage { get; set; }
		public UInt16[] Universes { get; set; }
		public override int Length => (UInt16)(MIN_LENGTH + (Universes.Length << 1));

		public static UniverseDiscoveryLayer Parse(ReadOnlySpan<byte> bytes)
		{
			UniverseDiscoveryLayer discoveryLayer = new UniverseDiscoveryLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			var pduLength = flagsAndLength & SACNLayer.LengthMask;
			bytes = bytes.Slice(sizeof(UInt16));

			discoveryLayer.Vector = BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));
			if (discoveryLayer.Vector != VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST)
			{
				return null;
			}

			discoveryLayer.Page = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			discoveryLayer.LastPage = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			var listLength = pduLength - 8;
			var universeCount = listLength / sizeof(UInt16);
			discoveryLayer.Universes = new UInt16[universeCount];

			for (int i = 0; i < universeCount; i++)
			{
				discoveryLayer.Universes[i] = BinaryPrimitives.ReadUInt16BigEndian(bytes);
				bytes = bytes.Slice(sizeof(UInt16));
			}

			return discoveryLayer;
		}
	}
}
