using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public enum UniverseDiscoveryLayerVector
	{
		VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST = 0x00000001
	}

	public class UniverseDiscoveryLayer : SACNLayer
	{
		public const int MinLength = 8;

		public UniverseDiscoveryLayerVector Vector { get; set; }
		public byte Page { get; set; }
		public byte LastPage { get; set; }
		public IEnumerable<UInt16> Universes { get; set; }
		public UInt16 Length => (UInt16)(MinLength + (Universes.Count() << 1));

		public UniverseDiscoveryLayer()
		{
			Vector = UniverseDiscoveryLayerVector.VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST;
		}

		public void Write(Span<byte> bytes)
		{
			BinaryPrimitives.WriteUInt16BigEndian(bytes, GetFlagsAndLength(Length));
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt32BigEndian(bytes, (UInt32)Vector);
			bytes = bytes.Slice(sizeof(UInt32));
			bytes[0] = Page;
			bytes = bytes.Slice(sizeof(byte));
			bytes[0] = LastPage;
			bytes = bytes.Slice(sizeof(byte));
			foreach (var universe in Universes)
			{
				BinaryPrimitives.WriteUInt16BigEndian(bytes, universe);
				bytes = bytes.Slice(sizeof(UInt16));
			}
		}

		public static UniverseDiscoveryLayer Parse(ReadOnlySpan<byte> bytes)
		{
			UniverseDiscoveryLayer discoveryLayer = new UniverseDiscoveryLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			var pduLength = flagsAndLength & SACNLayer.LengthMask;
			bytes = bytes.Slice(sizeof(UInt16));

			discoveryLayer.Vector = (UniverseDiscoveryLayerVector)BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));
			if (discoveryLayer.Vector != UniverseDiscoveryLayerVector.VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST)
			{
				return null;
			}

			discoveryLayer.Page = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			discoveryLayer.LastPage = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			var listLength = pduLength - 8;
			var universeCount = listLength / sizeof(UInt16);
			var universes = new UInt16[universeCount];

			for (int i = 0; i < universeCount; i++)
			{
				universes[i] = BinaryPrimitives.ReadUInt16BigEndian(bytes);
				bytes = bytes.Slice(sizeof(UInt16));
			}

			discoveryLayer.Universes = universes;

			return discoveryLayer;
		}
	}
}
