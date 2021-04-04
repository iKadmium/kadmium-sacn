using Kadmium_Udp;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SACNServerTests
	{
		[Fact]
		public void Given_TheServerIsListeningOnIPV6_When_JoinMulticastGroupIsCalled_Then_ItJoinsAnIPV6Group()
		{
			IPAddress joinedAddress = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Parse("::1"), 1234);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == endPoint
			);

			Mock.Get(udpWrapper)
				.Setup(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()))
				.Callback<IPAddress>(addr => joinedAddress = addr);

			SACNServer server = new SACNServer(udpWrapper);
			server.Listen();
			server.JoinMulticastGroup(1);

			Assert.Equal(AddressFamily.InterNetworkV6, joinedAddress.AddressFamily);
		}

		[Fact]
		public void Given_TheServerIsListeningOnIPV4_When_JoinMulticastGroupIsCalled_Then_ItJoinsAnIPV4Group()
		{
			IPAddress joinedAddress = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == endPoint
			);

			Mock.Get(udpWrapper)
				.Setup(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()))
				.Callback<IPAddress>(addr => joinedAddress = addr);

			SACNServer server = new SACNServer(udpWrapper);
			server.Listen();
			server.JoinMulticastGroup(1);

			Assert.Equal(AddressFamily.InterNetwork, joinedAddress.AddressFamily);
		}

		[Theory]
		[InlineData("::1", 1, "FF18::83:00:00:01")]
		[InlineData("::1", 256, "FF18::83:00:01:00")]
		[InlineData("::1", 63999, "FF18::83:00:F9:FF")]
		[InlineData("127.0.0.1", 1, "239.255.0.1")]
		[InlineData("127.0.0.1", 256, "239.255.1.0")]
		[InlineData("127.0.0.1", 63999, "239.255.249.255")]
		public void Given_TheServerIsListeningOnAGivenAddress_When_JoinMulticastGroupIsCalled_Then_ItJoinsTheCorrectAddress(string localAddress, UInt16 universe, string expectedAddressString)
		{
			IPAddress joinedAddress = null;
			EndPoint endPoint = new IPEndPoint(IPAddress.Parse(localAddress), 1234);

			var udpWrapper = Mock.Of<IUdpWrapper>(x =>
				x.HostEndPoint == endPoint
			);

			Mock.Get(udpWrapper)
				.Setup(x => x.JoinMulticastGroup(It.IsAny<IPAddress>()))
				.Callback<IPAddress>(addr => joinedAddress = addr);

			SACNServer server = new SACNServer(udpWrapper);
			server.Listen(IPAddress.Parse(localAddress));
			server.JoinMulticastGroup(universe);

			IPAddress expectedAddress = IPAddress.Parse(expectedAddressString);
			Assert.Equal(expectedAddress, joinedAddress);
		}
	}
}
