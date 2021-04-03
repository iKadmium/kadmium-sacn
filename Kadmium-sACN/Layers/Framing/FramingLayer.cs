using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers.Framing
{
	public enum FramingLayerVector
	{
		VECTOR_E131_DATA_PACKET = 0x00000002,
		VECTOR_E131_EXTENDED_SYNCHRONIZATION = 0x00000001,
		VECTOR_E131_EXTENDED_DISCOVERY = 0x00000002
	}

	public abstract class FramingLayer : SACNLayer
	{
		public FramingLayerVector Vector { get; set; }

		public static FramingLayer Parse(ReadOnlySpan<byte> bytes, RootLayerVector rootLayerVector)
		{
			var vector = (FramingLayerVector)BinaryPrimitives.ReadUInt32BigEndian(bytes.Slice(sizeof(UInt16)));

			switch (rootLayerVector)
			{
				case RootLayerVector.VECTOR_ROOT_E131_DATA:
					switch(vector)
					{
						case FramingLayerVector.VECTOR_E131_DATA_PACKET:
							return DataPacketFramingLayer.Parse(bytes);
					}
					break;
				case RootLayerVector.VECTOR_ROOT_E131_EXTENDED:
					switch(vector)
					{
						case FramingLayerVector.VECTOR_E131_EXTENDED_SYNCHRONIZATION:
							return SynchronizationPacketFramingLayer.Parse(bytes);
						case FramingLayerVector.VECTOR_E131_EXTENDED_DISCOVERY:
							return UniverseDiscoveryPacketFramingLayer.Parse(bytes);
					}
					break;
			}

			return null;
		}
	}
}
