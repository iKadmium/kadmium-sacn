using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public class UniverseDiscoveryPacketFramingLayer : FramingLayer
	{
		public string SourceName { get; set; }
		public override int Length => 74;

		public static UniverseDiscoveryPacketFramingLayer Parse(ReadOnlySpan<byte> bytes)
		{
			UniverseDiscoveryPacketFramingLayer framingLayer = new UniverseDiscoveryPacketFramingLayer();

			framingLayer.FlagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(0, framingLayer.FlagsAndLength);
			bytes = bytes.Slice(sizeof(UInt16));

			framingLayer.Vector = BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			var sourceNameBytes = bytes.Slice(0, 64);
			bytes = bytes.Slice(64);
			framingLayer.SourceName = Encoding.UTF8.GetString(sourceNameBytes.ToArray());

			return framingLayer;
		}
	}
}
