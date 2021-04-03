using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers.Framing
{
	public class SynchronizationPacketFramingLayer : FramingLayer
	{
		public const int LENGTH = 11;
		public byte SequenceNumber { get; set; }
		public UInt16 SynchronizationAddress { get; set; }
		public override int Length => LENGTH;

		public static SynchronizationPacketFramingLayer Parse(ReadOnlySpan<byte> bytes)
		{
			SynchronizationPacketFramingLayer framingLayer = new SynchronizationPacketFramingLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			framingLayer.Vector = (FramingLayerVector)BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			framingLayer.SequenceNumber = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			framingLayer.SynchronizationAddress = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			return framingLayer;
		}
	}
}
