using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers.Framing
{
	public class UniverseDiscoveryPacketFramingLayer : FramingLayer
	{
		public const int Length = 74;
		public string SourceName { get; set; }

		public UniverseDiscoveryPacketFramingLayer()
		{
			Vector = FramingLayerVector.VECTOR_E131_EXTENDED_DISCOVERY;
		}

		public void Write(Span<byte> bytes, ushort remainingLength)
		{
			UInt16 pduLength = (UInt16)(remainingLength + Length);
			BinaryPrimitives.WriteUInt16BigEndian(bytes, GetFlagsAndLength(pduLength));
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt32BigEndian(bytes, (UInt32)Vector);
			bytes = bytes.Slice(sizeof(UInt32));
			var sourceNameBytes = Encoding.UTF8.GetBytes(SourceName);
			sourceNameBytes.CopyTo(bytes);
			bytes = bytes.Slice(sourceNameBytes.Length);
			var paddingLength = 64 - sourceNameBytes.Length;
			bytes.Slice(0, paddingLength).Fill(0);
			bytes = bytes.Slice(paddingLength);
			bytes.Slice(0, 4).Fill(0); // reserved
		}

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
