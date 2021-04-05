using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class DataPacketTests
	{
		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			byte[] CID = Enumerable.Range(1, 12).Select(x => (byte)x).ToArray();
			string sourceName = "Source name";
			UInt16 universe = 1050;
			byte[] properties = Enumerable.Range(0, 200).Select(x => (byte)x).ToArray();

			DataPacket packet = new DataPacket();
			packet.RootLayer.CID = CID;
			packet.FramingLayer.SourceName = sourceName;
			packet.FramingLayer.Universe = universe;
			packet.DMPLayer.PropertyValues = properties;

			using var owner = MemoryPool<byte>.Shared.Rent(packet.Length);
			var bytes = owner.Memory.Span.Slice(0, packet.Length);
			packet.Write(bytes);

			var expectedBytes = GetDataPacket(CID, sourceName, universe, properties);
		}

		public static Span<byte> GetDataPacket(byte[] CID, string sourceName, UInt16 universe, byte[] properties)
		{
			var rootLayer = new RootLayer
			{
				CID = CID,
				Vector = RootLayerVector.VECTOR_ROOT_E131_DATA
			};

			var framingLayer = new DataPacketFramingLayer
			{
				SourceName = sourceName,
				Universe = universe
			};

			var dmpLayer = new DMPLayer
			{
				PropertyValues = properties
			};

			int totalLength = dmpLayer.Length + DataPacketFramingLayer.Length + RootLayer.Length;
			using var owner = MemoryPool<byte>.Shared.Rent(totalLength);
			var bytes = owner.Memory.Span.Slice(0, totalLength);

			rootLayer.Write(bytes, (UInt16)(DataPacketFramingLayer.Length + dmpLayer.Length));
			framingLayer.Write(bytes.Slice(RootLayer.Length), dmpLayer.Length);
			dmpLayer.Write(bytes.Slice(RootLayer.Length + DataPacketFramingLayer.Length));

			return bytes;
		}
	}
}
