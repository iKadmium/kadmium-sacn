using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public class UniverseDiscoveryPacket : SACNPacket
	{
		public UniverseDiscoveryPacketFramingLayer FramingLayer { get; set; }
		public UniverseDiscoveryLayer UniverseDiscoveryLayer { get; set; }

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
	}
}
