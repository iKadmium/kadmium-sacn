using Kadmium_sACN.SacnReceiver;
using Kadmium_Udp;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.SacnReceiver
{
	public class MulticastSacnReceiverIPV4Tests
	{
		[Fact]
		public void Given_TheAddressIsNotIPV4_When_ListenIsCalled_Then_AnExceptionIsThrown()
		{
			var receiver = new MulticastSacnReceiverIPV4(null, null);
			Assert.Throws<ArgumentException>(() => receiver.Listen(IPAddress.IPv6Any));
		}

		[Fact]
		public void Given_TheAddressIsIPV4_When_ListenIsCalled_Then_ItListensOnTheAddress()
		{
			var ip = IPAddress.Any;
			var udpPipeline = Mock.Of<IUdpPipeline>();
			var receiver = new MulticastSacnReceiverIPV4(udpPipeline, null);
			receiver.Listen(ip);
			Mock.Get(udpPipeline)
				.Verify(x => x.ListenAsync(
					It.IsAny<PipeWriter>(),
					It.Is<IPEndPoint>(e => e.Address.Equals(ip))
				));
		}
	}
}
