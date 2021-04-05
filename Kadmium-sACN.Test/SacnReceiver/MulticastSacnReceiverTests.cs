using Kadmium_sACN.Layers.Framing;
using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_sACN.SacnReceiver;
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

namespace Kadmium_sACN.Test.SacnReceiver
{
	public class MulticastSacnReceiverTests
	{
		private class TestMulticastSacnReceiver : MulticastSacnReceiver
		{
			public TestMulticastSacnReceiver(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpWrapper, multicastAddressProvider)
			{
			}

			public override void Listen(IPAddress address)
			{
				ListenInternal(address);
			}
		}

		[Fact]
		public void Given_TheServerIsListening_When_JoinMulticastGroupIsCalled_Then_ItJoinsAMulticastGroup()
		{
			IPAddress joinedAddress = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Parse("::1"), 1234);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == endPoint
			);

			var returnedAddress = new IPAddress(new byte[] { 192, 168, 0, 1 });

			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetMulticastAddress(It.IsAny<UInt16>()) == returnedAddress
			);

			Mock.Get(udpWrapper)
				.Setup(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()))
				.Callback<IPAddress>(addr => joinedAddress = addr);

			var server = new TestMulticastSacnReceiver(udpWrapper, addressProvider);
			server.Listen(IPAddress.Any);
			server.JoinMulticastGroup(1);

			Assert.Equal(returnedAddress, joinedAddress);
		}

		[Fact]
		public void Given_TheUniverseIsTooHigh_When_TheUniverseIsSet_Then_AnExceptionIsThrown()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			var server = new TestMulticastSacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
			server.Listen(IPAddress.Any);
			Assert.Throws<ArgumentOutOfRangeException>(() => server.JoinMulticastGroup(Constants.Universe_MaxValue + 1));
		}

		[Fact]
		public void Given_TheUniverseIsTooLow_When_TheUniverseIsSet_Then_AnExceptionIsThrown()
		{
			var udpWrapper = Mock.Of<IUdpWrapper>();
			var server = new TestMulticastSacnReceiver(udpWrapper, Mock.Of<ISacnMulticastAddressProvider>());
			server.Listen(IPAddress.Any);
			Assert.Throws<ArgumentOutOfRangeException>(() => server.JoinMulticastGroup(Constants.Universe_MinValue - 1));
		}

		[Fact]
		public void When_JoinMulticastGroupsIsCalled_Then_AllTheGroupsAreJoined()
		{
			var universes = Enumerable.Range(Constants.Universe_MinValue, 5)
				.Select(x => (UInt16)x);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == new IPEndPoint(new IPAddress(new byte[] { 192, 168, 0, 1 }), 5568)
			);
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>();
			
			var server = new TestMulticastSacnReceiver(udpWrapper, addressProvider);
			server.Listen(IPAddress.Any);
			server.JoinMulticastGroups(universes);

			Mock.Get(udpWrapper)
				.Verify(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()), Times.Exactly(universes.Count()));
		}

		
	}
}
