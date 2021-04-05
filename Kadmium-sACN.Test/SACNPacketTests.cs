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
	public class SacnPacketTests
	{
		[Fact]
		public void Given_ThePacketIsAValidDataPacket_When_ParseIsCalled_Then_ThePacketIsParsedCorrectly()
		{
			byte[] CID = Enumerable.Range(1, 12).Select(x => (byte)x).ToArray();
			string sourceName = "Source name";
			UInt16 universe = 1050;
			byte[] properties = Enumerable.Range(0, 200).Select(x => (byte)x).ToArray();

			var bytes = DataPacketTests.GetDataPacket(CID, sourceName, universe, properties);
			
			var packet = SacnPacket.Parse(bytes);

			Assert.NotNull(packet);
			Assert.IsType<DataPacket>(packet);
		}

		[Fact]
		public void Given_ThePacketIsAValidUniverseDiscoveryPacket_When_ParseIsCalled_Then_ThePacketIsParsedCorrectly()
		{
			byte[] CID = Enumerable.Range(1, 12).Select(x => (byte)x).ToArray();
			var rootLayer = new RootLayer
			{
				CID = CID,
				Vector = RootLayerVector.VECTOR_ROOT_E131_EXTENDED
			};

			string sourceName = "Source name";
			
			var framingLayer = new UniverseDiscoveryPacketFramingLayer
			{
				SourceName = sourceName
			};

			UInt16[] universes = Enumerable.Range(200, 1000)
				.Select(x => (UInt16)x)
				.ToArray();

			var discoveryLayer = new UniverseDiscoveryLayer
			{
				Universes = universes,
				Page = 25,
				LastPage = 25
			};

			int totalLength = discoveryLayer.Length + UniverseDiscoveryPacketFramingLayer.Length + RootLayer.Length;
			using var owner = MemoryPool<byte>.Shared.Rent(totalLength);
			var bytes = owner.Memory.Span.Slice(0, totalLength);

			rootLayer.Write(bytes, (UInt16)(UniverseDiscoveryPacketFramingLayer.Length + discoveryLayer.Length));
			framingLayer.Write(bytes.Slice(RootLayer.Length), discoveryLayer.Length);
			discoveryLayer.Write(bytes.Slice(RootLayer.Length + UniverseDiscoveryPacketFramingLayer.Length));

			var packet = SacnPacket.Parse(bytes);

			Assert.NotNull(packet);
			Assert.IsType<UniverseDiscoveryPacket>(packet);
		}

		[Fact]
		public void Given_ThePacketIsAValidSynchronizationPacket_When_ParseIsCalled_Then_ThePacketIsParsedCorrectly()
		{
			byte[] CID = Enumerable.Range(1, 12).Select(x => (byte)x).ToArray();
			byte sequenceNumber = 124;
			var bytes = SynchronizationPacketTests.GetSynchronizationPacket(CID, sequenceNumber);
			
			var packet = SacnPacket.Parse(bytes);

			Assert.NotNull(packet);
			Assert.IsType<SynchronizationPacket>(packet);
		}
	}
}
