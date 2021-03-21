using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public abstract class FramingLayer : SACNLayer
	{
		public const UInt32 VECTOR_E131_DATA_PACKET =				0x00000002;
		public const UInt32 VECTOR_E131_EXTENDED_SYNCHRONIZATION =	0x00000001;
		public const UInt32 VECTOR_E131_EXTENDED_DISCOVERY =		0x00000002;

		public const byte PreviewDataMask  =			0b00000010;
		public const byte StreamTerminatedMask =		0b00000100;
		public const byte ForceSynchronizationMask =	0b00001000;

		public UInt32 Vector { get; set; }
		
		public static FramingLayer Parse(ReadOnlySpan<byte> bytes, UInt32 rootLayerVector)
		{
			var vector = BinaryPrimitives.ReadUInt32BigEndian(bytes.Slice(sizeof(UInt16)));

			if (rootLayerVector == RootLayer.VECTOR_ROOT_E131_DATA)
			{
				if (vector == VECTOR_E131_DATA_PACKET)
				{
					return DataPacketFramingLayer.Parse(bytes);
				}
			}

			switch (rootLayerVector)
			{
				case RootLayer.VECTOR_ROOT_E131_DATA:
					switch(vector)
					{
						case VECTOR_E131_DATA_PACKET:
							return DataPacketFramingLayer.Parse(bytes);
					}
					break;
				case RootLayer.VECTOR_ROOT_E131_EXTENDED:
					switch(vector)
					{
						case VECTOR_E131_EXTENDED_SYNCHRONIZATION:
							break;
						case VECTOR_E131_EXTENDED_DISCOVERY:
							break;
					}
					break;
			}

			throw new ArgumentException($"Invalid combination of root layer vector {rootLayerVector} and framing layer vector {vector}");
		}
	}
}
