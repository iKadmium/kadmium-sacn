using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public class SynchronizationPacket : SacnPacket
	{
		public SynchronizationPacketFramingLayer FramingLayer { get; set; }
		public override int Length => RootLayer.Length + SynchronizationPacketFramingLayer.Length;

		public SynchronizationPacket()
		{
			RootLayer = new RootLayer
			{
				Vector = RootLayerVector.VECTOR_ROOT_E131_EXTENDED
			};
			FramingLayer = new SynchronizationPacketFramingLayer();
		}

		public static SynchronizationPacket Parse(ReadOnlySpan<byte> bytes, RootLayer rootLayer, SynchronizationPacketFramingLayer framingLayer)
		{
			SynchronizationPacket packet = new SynchronizationPacket
			{
				RootLayer = rootLayer,
				FramingLayer = framingLayer
			};

			return packet;
		}

		public override void Write(Span<byte> bytes)
		{
			RootLayer.Write(bytes, SynchronizationPacketFramingLayer.Length);
			FramingLayer.Write(bytes.Slice(RootLayer.Length));
		}
	}
}
