using Kadmium_sACN.SacnReceiver;
using Kadmium_Udp;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.SacnReceiver
{
	public class MulticastSacnReceiverIPV6Tests
	{
		[Fact]
		public void Given_TheAddressIsNotIPV6_When_ListenIsCalled_Then_AnExceptionIsThrown()
		{
			var receiver = new MulticastSacnReceiverIPV6(null, null);
			Assert.Throws<ArgumentException>(() => receiver.Listen(IPAddress.Any));
		}

		[Fact]
		public void Given_TheAddressIsIPV6_When_ListenIsCalled_Then_ItListensOnTheAddress()
		{
			var ip = IPAddress.IPv6Any;
			var udpWrapper = Mock.Of<IUdpWrapper>();
			var receiver = new MulticastSacnReceiverIPV6(udpWrapper, null);
			receiver.Listen(ip);
			Mock.Get(udpWrapper)
				.Verify(x => x.Listen(It.Is<IPEndPoint>(e => e.Address.Equals(ip))));
		}
	}
}
