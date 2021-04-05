using Kadmium_sACN.SacnSender;
using Kadmium_Udp;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.SacnSender
{
	public class UnicastSenderTests
	{
		[Fact]
		public async Task When_UnicastSendIsCalled_Then_ThePacketIsSent()
		{
			var expectedAddress = new IPAddress(new byte[] { 1, 2, 3, 4 });
			var packet = new DataPacket();
			packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };
			packet.FramingLayer.SourceName = "Source";
			byte[] expectedBytes = new byte[packet.Length];
			packet.Write(expectedBytes);

			var udpWrapper = Mock.Of<IUdpWrapper>();
			var sender = new UnicastSacnSender(udpWrapper, expectedAddress);
			await sender.Send(packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedAddress && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}
	}
}
