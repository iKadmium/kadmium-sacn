using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public class DataPacket : SACNPacket
	{
		public DataPacketFramingLayer FramingLayer { get; set; }
		public DMPLayer DMPLayer { get; set; }

		public byte[] Properties => DMPLayer.PropertyValues;

		public DataPacket()
		{
			RootLayer = new RootLayer();
			FramingLayer = new DataPacketFramingLayer(); 
			DMPLayer = new DMPLayer();
		}

		public static DataPacket Parse(ReadOnlySpan<byte> bytes, RootLayer rootLayer, DataPacketFramingLayer framingLayer)
		{
			DataPacket dataPacket = new DataPacket
			{
				RootLayer = rootLayer,
				FramingLayer = framingLayer,
				DMPLayer = DMPLayer.Parse(bytes)
			};

			return dataPacket;
		}
	}
}
