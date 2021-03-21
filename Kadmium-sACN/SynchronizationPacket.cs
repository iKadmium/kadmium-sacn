using Kadmium_sACN.Layers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public class SynchronizationPacket : SACNPacket
	{
		public SynchronizationPacketFramingLayer FramingLayer { get; set; }

		public static SynchronizationPacket Parse(ReadOnlySpan<byte> bytes, RootLayer rootLayer, SynchronizationPacketFramingLayer framingLayer)
		{
			SynchronizationPacket packet = new SynchronizationPacket
			{
				RootLayer = rootLayer,
				FramingLayer = framingLayer
			};

			return packet;
		}
	}
}
