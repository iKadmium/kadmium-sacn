using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers.Framing
{
	public class SynchronizationPacketFramingLayer : FramingLayer
	{
		public const int Length = 11;
		public byte SequenceNumber { get; set; }
		public UInt16 SynchronizationAddress { get; set; }

		public SynchronizationPacketFramingLayer()
		{
			Vector = FramingLayerVector.VECTOR_E131_EXTENDED_SYNCHRONIZATION;
		}

		public void Write(Span<byte> bytes)
		{
			UInt16 pduLength = (UInt16)(Length);
			BinaryPrimitives.WriteUInt16BigEndian(bytes, GetFlagsAndLength(pduLength));
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt32BigEndian(bytes, (UInt32)Vector);
			bytes = bytes.Slice(sizeof(UInt32));
			bytes[0] = SequenceNumber;
			bytes = bytes.Slice(sizeof(byte));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, SynchronizationAddress);
			bytes = bytes.Slice(sizeof(UInt16));
			bytes.Slice(0, 2).Fill(0); // reserved
		}

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
