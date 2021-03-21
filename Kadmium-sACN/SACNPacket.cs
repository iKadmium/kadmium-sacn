using Kadmium_sACN.Layers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public abstract class SACNPacket
	{
		public RootLayer RootLayer { get; set; }

		public SACNPacket Parse(ReadOnlySpan<byte> bytes)
		{
			RootLayer rootLayer = RootLayer.Parse(bytes);
			bytes = bytes.Slice(rootLayer.Length);

			FramingLayer framingLayer = FramingLayer.Parse(bytes, rootLayer.Vector);
			bytes = bytes.Slice(framingLayer.Length);

			switch(framingLayer)
			{
				case DataPacketFramingLayer dataLayer:
					return DataPacket.Parse(bytes, rootLayer, dataLayer);
				case SynchronizationPacketFramingLayer syncLayer:
					return SynchronizationPacket.Parse(bytes, rootLayer, syncLayer);
				case UniverseDiscoveryPacketFramingLayer discoveryLayer:
					return UniverseDiscoveryPacket.Parse(bytes, rootLayer, discoveryLayer);
				default:
					throw new ArgumentException("Unable to parse given packet");
			}
		}
	}
}
