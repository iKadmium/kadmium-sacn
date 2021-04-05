using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SacnPacketFactoryTests
	{
		[Fact]
		public void When_NewDataPacketsAreCreated_Then_TheirSequenceNumbersAreIncremented()
		{
			var sacnPacketFactory = GetPacketFactory();
			var first = sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 });
			var second = sacnPacketFactory.CreateDataPacket(1, new byte[] { 255 });

			var expectedFirstSequenceNumber = 0;
			var expectedSecondSequenceNumber = 1;

			Assert.Equal(expectedFirstSequenceNumber, first.FramingLayer.SequenceNumber);
			Assert.Equal(expectedSecondSequenceNumber, second.FramingLayer.SequenceNumber);
		}

		[Fact]
		public void Given_TheSequenceNumberIs255_When_NewDataPacketsAreCreated_Then_TheirSequenceNumbersAreReset()
		{
			var sacnPacketFactory = GetPacketFactory();
			for(int i = 0; i < 255; i++)
			{
				sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 });
			}
			var first = sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 });
			var second = sacnPacketFactory.CreateDataPacket(1, new byte[] { 255 });

			var expectedFirstSequenceNumber = 255;
			var expectedSecondSequenceNumber = 0;

			Assert.Equal(expectedFirstSequenceNumber, first.FramingLayer.SequenceNumber);
			Assert.Equal(expectedSecondSequenceNumber, second.FramingLayer.SequenceNumber);
		}

		[Fact]
		public void When_NewDataPacketsAreCreated_Then_TheCIDIsSet()
		{
			var cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			var sourceName = "This is a source";
			var sacnPacketFactory = new SacnPacketFactory(cid, sourceName);
			var packet = sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 });
			
			Assert.Equal(cid, packet.RootLayer.CID);
		}

		[Fact]
		public void When_NewDataPacketsAreCreated_Then_TheSourceNameIsSet()
		{
			var cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			var sourceName = "This is a source";
			var sacnPacketFactory = new SacnPacketFactory(cid, sourceName);
			var packet = sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 });

			Assert.Equal(sourceName, packet.FramingLayer.SourceName);
		}

		[Fact]
		public void When_NewDataPacketsAreCreated_Then_TheUniverseIsSet()
		{
			var expectedUniverse = (UInt16)25;
			var sacnPacketFactory = GetPacketFactory();
			var packet = sacnPacketFactory.CreateDataPacket(expectedUniverse, new byte[] { 0 });

			Assert.Equal(expectedUniverse, packet.FramingLayer.Universe);
		}

		[Fact]
		public void When_NewDataPacketsAreCreated_Then_ThePriorityIsSet()
		{
			var expectedPriority = (byte)195;
			var sacnPacketFactory = GetPacketFactory();
			var packet = sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 }, priority: expectedPriority);

			Assert.Equal(expectedPriority, packet.FramingLayer.Priority);
		}

		[Fact]
		public void When_NewDataPacketsAreCreated_Then_TheStartCodeIsSet()
		{
			var expectedStartCode = (byte)202;
			var sacnPacketFactory = GetPacketFactory();
			var packet = sacnPacketFactory.CreateDataPacket(1, new byte[] { 0 }, startCode: expectedStartCode);

			Assert.Equal(expectedStartCode, packet.DMPLayer.StartCode);
		}

		[Fact]
		public void When_NewDataPacketsAreCreated_Then_ThePropertiesAreSet()
		{
			var expectedProperties = Enumerable.Range(0, 255)
				.Select(x => (byte)x);

			var sacnPacketFactory = GetPacketFactory();
			var packet = sacnPacketFactory.CreateDataPacket(1, expectedProperties);

			Assert.Equal(expectedProperties, packet.DMPLayer.PropertyValues);
		}

		[Theory]
		[InlineData(UniverseDiscoveryPacket.PageSize, 1)]
		[InlineData(UniverseDiscoveryPacket.PageSize + 1, 2)]
		[InlineData(63999, 125)]
		public void When_NewDiscoveryPacketsAreCreated_Then_TheNumberOfPacketsIsAsExpected(int universeCount, int expectedPacketCount)
		{
			var sacnPacketFactory = GetPacketFactory();
			var universes = Enumerable.Range(1, universeCount)
				.Select(x => (UInt16)x);
			var packets = sacnPacketFactory.CreateUniverseDiscoveryPackets(universes);

			Assert.Equal(expectedPacketCount, packets.Count());
		}

		private static SacnPacketFactory GetPacketFactory()
		{
			var cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			var sourceName = "This is a source";
			var sacnPacketFactory = new SacnPacketFactory(cid, sourceName);
			return sacnPacketFactory;
		}
	}
}
