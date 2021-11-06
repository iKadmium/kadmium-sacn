using Kadmium_Udp;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.SacnReceiver
{
	public class SacnReceiverTests
	{
		private class TestSacnReceiver : Kadmium_sACN.SacnReceiver.SacnReceiver
		{
			public TestSacnReceiver(IUdpPipeline udpPipeline) : base(udpPipeline)
			{
			}

			public override void Listen(IPAddress address)
			{
				ListenInternal(address);
			}
		}

		[Fact]
		public void When_TheServerIsDisposed_Then_TheudpPipelineIsDisposed()
		{
			var udpPipeline = Mock.Of<IUdpPipeline>();
			var server = new TestSacnReceiver(udpPipeline);
			server.Dispose();
			Mock.Get(udpPipeline)
				.Verify(x => x.Dispose());
		}

		[Fact]
		public async Task When_ADataPacketIsReceived_Then_TheEventIsTriggered()
		{
			PipeWriter writer = null;
			var udpPipeline = Mock.Of<IUdpPipeline>();

			Mock.Get(udpPipeline)
				.Setup(x => x.ListenAsync(It.IsAny<PipeWriter>(), It.IsAny<IPEndPoint>()))
				.Callback<PipeWriter, IPEndPoint>((pipeWriter, endPoint) => writer = pipeWriter);

			var server = new TestSacnReceiver(udpPipeline);
			server.Listen(IPAddress.Any);
			DataPacket receivedPacket = null;

			server.OnDataPacketReceived += (object sender, DataPacket packet) =>
			{
				receivedPacket = packet;
			};

			byte[] CID = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			string sourceName = "This is a good enough source name";
			UInt16 universe = 1234;
			byte[] properties = Enumerable.Range(1, 25)
				.Select(x => (byte)x)
				.ToArray();

			var buffer = DataPacketTests.GetDataPacket(CID, sourceName, universe, properties).ToArray();

			await writer.WriteAsync(buffer);
			await Task.Delay(250);
			Assert.NotNull(receivedPacket);
		}

		[Fact]
		public async Task When_ASynchronizationPacketIsReceived_Then_TheEventIsTriggered()
		{
			PipeWriter writer = null;
			var udpPipeline = Mock.Of<IUdpPipeline>();

			Mock.Get(udpPipeline)
				.Setup(x => x.ListenAsync(It.IsAny<PipeWriter>(), It.IsAny<IPEndPoint>()))
				.Callback<PipeWriter, IPEndPoint>((pipeWriter, endPoint) => writer = pipeWriter);

			var server = new TestSacnReceiver(udpPipeline);
			server.Listen(IPAddress.Any);
			SynchronizationPacket receivedPacket = null;

			server.OnSynchronizationPacketReceived += (object sender, SynchronizationPacket packet) =>
			{
				receivedPacket = packet;
			};

			byte[] CID = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			byte sequenceNumber = 43;

			var buffer = SynchronizationPacketTests.GetSynchronizationPacket(CID, sequenceNumber).ToArray();
			await writer.WriteAsync(buffer);
			await Task.Delay(250);
			Assert.NotNull(receivedPacket);
		}

		[Fact]
		public async Task When_AUniverseDiscoveryPacketISReceived_Then_TheEventIsTriggered()
		{
			PipeWriter writer = null;
			var udpPipeline = Mock.Of<IUdpPipeline>();

			Mock.Get(udpPipeline)
				.Setup(x => x.ListenAsync(It.IsAny<PipeWriter>(), It.IsAny<IPEndPoint>()))
				.Callback<PipeWriter, IPEndPoint>((pipeWriter, endPoint) => writer = pipeWriter);

			var server = new TestSacnReceiver(udpPipeline);
			server.Listen(IPAddress.Any);
			UniverseDiscoveryPacket receivedPacket = null;

			server.OnUniverseDiscoveryPacketReceived += (object sender, UniverseDiscoveryPacket packet) =>
			{
				receivedPacket = packet;
			};

			byte[] CID = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			string sourceName = "This is a good enough source name";
			byte page = 57;
			byte lastPage = 59;
			UInt16[] universes = Enumerable.Range(1200, 25)
				.Select(x => (UInt16)x)
				.ToArray();

			var buffer = UniverseDiscoveryPacketTests.GetUniverseDiscoveryPacket(CID, sourceName, page, lastPage, universes).ToArray();
			await writer.WriteAsync(buffer);

			await Task.Delay(250);
			Assert.NotNull(receivedPacket);
		}
	}
}
