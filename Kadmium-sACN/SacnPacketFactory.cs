using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Kadmium_sACN
{
	public class SacnPacketFactory
	{
		public byte[] CID { get; set; }
		public string SourceName { get; set; }
		private byte SequenceNumber { get; set; }
		
		public SacnPacketFactory(byte[] cid, string sourceName)
		{
			CID = cid;
			SourceName = sourceName;
		}

		public DataPacket CreateDataPacket(UInt16 universe, IEnumerable<byte> properties, byte priority = Constants.Priority_Default, byte startCode = 0)
		{
			DataPacket dataPacket = new DataPacket();
			dataPacket.RootLayer.CID = CID;
			dataPacket.FramingLayer.SourceName = SourceName;
			dataPacket.FramingLayer.SequenceNumber = SequenceNumber;
			dataPacket.FramingLayer.Universe = universe;
			dataPacket.FramingLayer.Priority = priority;
			dataPacket.DMPLayer.StartCode = startCode;
			dataPacket.DMPLayer.PropertyValues = properties;

			SequenceNumber = (SequenceNumber == byte.MaxValue) ? byte.MinValue : (byte)(SequenceNumber + 1);

			return dataPacket;
		}

		public IEnumerable<UniverseDiscoveryPacket> CreateUniverseDiscoveryPackets(IEnumerable<UInt16> universes)
		{
			var chunks = universes.Chunk(UniverseDiscoveryPacket.PageSize);

			byte page = 0;
			byte lastPage = (byte)chunks.Count();

			foreach(var chunk in chunks)
			{
				yield return CreateUniverseDiscoveryPage(chunk, page, lastPage);
				page++;
			}
		}

		private UniverseDiscoveryPacket CreateUniverseDiscoveryPage(IEnumerable<UInt16> universes, byte page, byte lastPage)
		{
			UniverseDiscoveryPacket packet = new UniverseDiscoveryPacket();
			packet.RootLayer.CID = CID;
			packet.FramingLayer.SourceName = SourceName;
			packet.UniverseDiscoveryLayer.Page = page;
			packet.UniverseDiscoveryLayer.LastPage = lastPage;
			packet.UniverseDiscoveryLayer.Universes = universes.ToArray();
			return packet;
		}
	}
}
