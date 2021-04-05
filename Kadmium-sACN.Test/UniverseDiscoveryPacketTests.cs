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
	public class UniverseDiscoveryPacketTests
	{
		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			byte[] CID = Enumerable.Range(1, 12).Select(x => (byte)x).ToArray();
			var sourceName = "This is a good enough source name";
			byte page = 1;
			byte lastPage = 45;
			UInt16[] universes = Enumerable.Range(25, 25)
				.Select(x => (UInt16)x)
				.ToArray();

			var packet = new UniverseDiscoveryPacket();
			packet.RootLayer.CID = CID;
			packet.FramingLayer.SourceName = sourceName;
			packet.UniverseDiscoveryLayer.Page = page;
			packet.UniverseDiscoveryLayer.LastPage = page;
			packet.UniverseDiscoveryLayer.Universes = universes;

			var expectedBytes = GetUniverseDiscoveryPacket(CID, sourceName, page, lastPage, universes);

			using var owner = MemoryPool<byte>.Shared.Rent(packet.Length);
			var bytes = owner.Memory.Span.Slice(0, packet.Length);
			packet.Write(bytes);

			Assert.Equal(expectedBytes.ToArray(), bytes.ToArray());

		}

		public static Span<byte> GetUniverseDiscoveryPacket(byte[] CID, string sourceName, byte page, byte lastPage, UInt16[] universes)
		{
			var rootLayer = new RootLayer
			{
				CID = CID,
				Vector = RootLayerVector.VECTOR_ROOT_E131_EXTENDED
			};

			var framingLayer = new UniverseDiscoveryPacketFramingLayer
			{
				SourceName = sourceName
			};

			var universeDiscoveryLayer = new UniverseDiscoveryLayer
			{
				Page = page,
				LastPage = lastPage,
				Universes = universes
			};


			int totalLength = RootLayer.Length + UniverseDiscoveryPacketFramingLayer.Length + universeDiscoveryLayer.Length;
			using var owner = MemoryPool<byte>.Shared.Rent(totalLength);
			var bytes = owner.Memory.Span.Slice(0, totalLength);

			rootLayer.Write(bytes, (UInt16)(SynchronizationPacketFramingLayer.Length + universeDiscoveryLayer.Length));
			framingLayer.Write(bytes.Slice(RootLayer.Length), universeDiscoveryLayer.Length);
			universeDiscoveryLayer.Write(bytes.Slice(RootLayer.Length + UniverseDiscoveryPacketFramingLayer.Length));

			return bytes;
		}
	}
}
