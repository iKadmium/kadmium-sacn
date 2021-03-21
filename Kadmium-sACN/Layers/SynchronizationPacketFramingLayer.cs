using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public class SynchronizationPacketFramingLayer : FramingLayer
	{
		public byte SequenceNumber { get; set; }
		public UInt16 SynchronizationAddress { get; set; }
		public override int Length => 11;

		public static SynchronizationPacketFramingLayer Parse(ReadOnlySpan<byte> bytes)
		{
			SynchronizationPacketFramingLayer framingLayer = new SynchronizationPacketFramingLayer();

			framingLayer.FlagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(0, framingLayer.FlagsAndLength);
			bytes = bytes.Slice(sizeof(UInt16));

			framingLayer.Vector = BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			framingLayer.SequenceNumber = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			framingLayer.SynchronizationAddress = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			return framingLayer;
		}
	}
}
