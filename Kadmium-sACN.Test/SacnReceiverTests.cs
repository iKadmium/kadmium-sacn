using Kadmium_Udp;
using Moq;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SacnReceiverTests
	{
		[Fact]
		public void Given_TheServerIsListeningOnIPV6_When_JoinMulticastGroupIsCalled_Then_ItJoinsAnIPV6Group()
		{
			IPAddress joinedAddress = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Parse("::1"), 1234);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == endPoint
			);

			var returnedAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });

			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetIPV6MulticastAddress(It.IsAny<UInt16>()) == returnedAddress
			);

			Mock.Get(udpWrapper)
				.Setup(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()))
				.Callback<IPAddress>(addr => joinedAddress = addr);

			SacnReceiver server = new SacnReceiver(udpWrapper, addressProvider);
			server.Listen(IPAddress.Any);
			server.JoinMulticastGroup(1);

			Assert.Equal(returnedAddress, joinedAddress);
		}

		[Fact]
		public void Given_TheServerIsListeningOnIPV4_When_JoinMulticastGroupIsCalled_Then_ItJoinsAnIPV4Group()
		{
			IPAddress joinedAddress = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == endPoint
			);

			var returnedAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });

			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetIPV4MulticastAddress(It.IsAny<UInt16>()) == returnedAddress
			);

			Mock.Get(udpWrapper)
				.Setup(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()))
				.Callback<IPAddress>(addr => joinedAddress = addr);

			SacnReceiver server = new SacnReceiver(udpWrapper, addressProvider);
			server.Listen(IPAddress.Any);
			server.JoinMulticastGroup(1);

			Assert.Equal(returnedAddress, joinedAddress);
		}

		

		[Fact]
		public void Given_TheUniverseIsTooHigh_When_TheUniverseIsSet_Then_AnExceptionIsThrown()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			SacnReceiver server = new SacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
			server.Listen(IPAddress.Any);
			Assert.Throws<ArgumentOutOfRangeException>(() => server.JoinMulticastGroup(SacnReceiver.Universe_MaxValue + 1));
		}

		[Fact]
		public void Given_TheUniverseIsTooLow_When_TheUniverseIsSet_Then_AnExceptionIsThrown()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			SacnReceiver server = new SacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
			server.Listen(IPAddress.Any);
			Assert.Throws<ArgumentOutOfRangeException>(() => server.JoinMulticastGroup(SacnReceiver.Universe_MinValue - 1));
		}

		[Fact]
		public void When_JoinMulticastGroupsIsCalled_Then_AllTheGroupsAreJoined()
		{
			var universes = Enumerable.Range(SacnReceiver.Universe_MinValue, 5)
				.Select(x => (UInt16)x);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == new IPEndPoint(new IPAddress(new byte[] { 192, 168, 0, 1 }), 5568)
			);
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>();
			
			SacnReceiver server = new SacnReceiver(udpWrapper, addressProvider);
			server.Listen(IPAddress.Any);
			server.JoinMulticastGroups(universes);

			Mock.Get(udpWrapper)
				.Verify(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()), Times.Exactly(universes.Count()));
		}

		[Fact]
		public void When_TheServerIsDisposed_Then_TheUdpWrapperIsDisposed()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			SacnReceiver server = new SacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
			server.Dispose();
			Mock.Get(udpWrapper)
				.Verify(x => x.Dispose());
		}

		[Fact]
		public async Task When_ADataPacketIsReceived_Then_TheEventIsTriggered()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			SacnReceiver server = new SacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
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

			Mock.Get(udpWrapper).Raise(x => x.OnPacketReceived += null, null, new UdpReceiveResult(buffer, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234)));

			await Task.Delay(250);
			Assert.NotNull(receivedPacket);
		}

		[Fact]
		public async Task When_ASynchronizationPacketIsReceived_Then_TheEventIsTriggered()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			SacnReceiver server = new SacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
			server.Listen(IPAddress.Any);
			SynchronizationPacket receivedPacket = null;

			server.OnSynchronizationPacketReceived += (object sender, SynchronizationPacket packet) =>
			{
				receivedPacket = packet;
			};

			byte[] CID = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
			byte sequenceNumber = 43;

			var buffer = SynchronizationPacketTests.GetSynchronizationPacket(CID, sequenceNumber).ToArray();

			Mock.Get(udpWrapper).Raise(x => x.OnPacketReceived += null, null, new UdpReceiveResult(buffer, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234)));

			await Task.Delay(250);
			Assert.NotNull(receivedPacket);
		}

		[Fact]
		public async Task When_AUniverseDiscoveryPacketISReceived_Then_TheEventIsTriggered()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			SacnReceiver server = new SacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
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

			Mock.Get(udpWrapper).Raise(x => x.OnPacketReceived += null, null, new UdpReceiveResult(buffer, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234)));

			await Task.Delay(250);
			Assert.NotNull(receivedPacket);
		}
	}
}
