using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers.Framing
{
	public class UniverseDiscoveryPacketFramingLayer : FramingLayer
	{
		public const int LENGTH = 74;
		public string SourceName { get; set; }
		public override int Length => LENGTH;
		public Func<UInt16> GetUniverseDiscoveryLayerLength { get; set; }

		public static UniverseDiscoveryPacketFramingLayer Parse(ReadOnlySpan<byte> bytes)
		{
			UniverseDiscoveryPacketFramingLayer framingLayer = new UniverseDiscoveryPacketFramingLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			framingLayer.Vector = (FramingLayerVector)BinaryPrimitives.ReadUInt32BigEndian(bytes);
			if (framingLayer.Vector != FramingLayerVector.VECTOR_E131_EXTENDED_DISCOVERY)
			{
				return null;
			}
			bytes = bytes.Slice(sizeof(UInt32));

			var sourceNameBytes = bytes.Slice(0, 64);
			bytes = bytes.Slice(64);
			var endOfString = sourceNameBytes.IndexOf((byte)0);
			framingLayer.SourceName = Encoding.UTF8.GetString(sourceNameBytes.Slice(0, endOfString).ToArray());

			return framingLayer;
		}
	}
}
