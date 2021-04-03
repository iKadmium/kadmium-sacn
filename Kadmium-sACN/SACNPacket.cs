using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public abstract class SACNPacket
	{
		public RootLayer RootLayer { get; set; }

		public static SACNPacket Parse(ReadOnlySpan<byte> bytes)
		{
			RootLayer rootLayer = RootLayer.Parse(bytes);
			if (rootLayer == null)
			{
				return null;
			}
			bytes = bytes.Slice(RootLayer.Length);

			FramingLayer framingLayer = FramingLayer.Parse(bytes, rootLayer.Vector);
			
			switch(framingLayer)
			{
				case DataPacketFramingLayer dataLayer:
					bytes = bytes.Slice(DataPacketFramingLayer.Length);
					return DataPacket.Parse(bytes, rootLayer, dataLayer);
				case SynchronizationPacketFramingLayer syncLayer:
					bytes = bytes.Slice(SynchronizationPacketFramingLayer.Length);
					return SynchronizationPacket.Parse(bytes, rootLayer, syncLayer);
				case UniverseDiscoveryPacketFramingLayer discoveryLayer:
					bytes = bytes.Slice(UniverseDiscoveryPacketFramingLayer.Length);
					return UniverseDiscoveryPacket.Parse(bytes, rootLayer, discoveryLayer);
				default:
					return null;
			}
		}
	}
}
