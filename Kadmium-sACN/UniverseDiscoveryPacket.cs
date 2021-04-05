using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public class UniverseDiscoveryPacket : SacnPacket
	{
		public UniverseDiscoveryPacketFramingLayer FramingLayer { get; set; }
		public UniverseDiscoveryLayer UniverseDiscoveryLayer { get; set; }
		public override int Length => RootLayer.Length + UniverseDiscoveryPacketFramingLayer.Length + UniverseDiscoveryLayer.Length;

		public UniverseDiscoveryPacket()
		{
			RootLayer = new RootLayer
			{
				Vector = RootLayerVector.VECTOR_ROOT_E131_EXTENDED
			};
			FramingLayer = new UniverseDiscoveryPacketFramingLayer();
			UniverseDiscoveryLayer = new UniverseDiscoveryLayer();
		}

		public static UniverseDiscoveryPacket Parse(ReadOnlySpan<byte> bytes, RootLayer rootLayer, UniverseDiscoveryPacketFramingLayer framingLayer)
		{
			UniverseDiscoveryPacket dataPacket = new UniverseDiscoveryPacket
			{
				RootLayer = rootLayer,
				FramingLayer = framingLayer,
				UniverseDiscoveryLayer = UniverseDiscoveryLayer.Parse(bytes)
			};

			return dataPacket;
		}

		public override void Write(Span<byte> bytes)
		{
			RootLayer.Write(bytes, (UInt16)(UniverseDiscoveryPacketFramingLayer.Length + UniverseDiscoveryLayer.Length));
			FramingLayer.Write(bytes.Slice(RootLayer.Length), UniverseDiscoveryLayer.Length);
			UniverseDiscoveryLayer.Write(bytes.Slice(RootLayer.Length + UniverseDiscoveryPacketFramingLayer.Length));
		}
	}
}
