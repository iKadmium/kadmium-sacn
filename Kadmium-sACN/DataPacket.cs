using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public class DataPacket : SacnPacket
	{
		public DataPacketFramingLayer FramingLayer { get; set; }
		public DMPLayer DMPLayer { get; set; }
		public override int Length => RootLayer.Length + DataPacketFramingLayer.Length + DMPLayer.Length;

		public DataPacket()
		{
			RootLayer = new RootLayer
			{
				Vector = RootLayerVector.VECTOR_ROOT_E131_DATA
			};
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

		public override void Write(Span<byte> bytes)
		{
			RootLayer.Write(bytes, (UInt16)(DataPacketFramingLayer.Length + DMPLayer.Length));
			FramingLayer.Write(bytes.Slice(RootLayer.Length), (UInt16)(DMPLayer.Length));
			DMPLayer.Write(bytes.Slice(RootLayer.Length + DataPacketFramingLayer.Length));
		}
	}
}
