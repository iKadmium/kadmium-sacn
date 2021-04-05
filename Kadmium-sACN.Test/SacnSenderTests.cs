using Kadmium_Udp;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SacnSenderTests
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
			SacnSender sender = new SacnSender(udpWrapper, null);
			await sender.UnicastSend(expectedAddress, packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedAddress && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}

		[Fact]
		public async Task When_MulticastSendIPV4IsCalled_Then_ThePacketIsSent()
		{
			var expectedUniverse = (UInt16)1234;
			var packet = new DataPacket();
			packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };
			packet.FramingLayer.SourceName = "Source";
			byte[] expectedBytes = new byte[packet.Length];
			packet.Write(expectedBytes);

			var expectedIP = IPAddress.Parse("127.0.0.1");

			var udpWrapper = Mock.Of<IUdpWrapper>();
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetIPV4MulticastAddress(
					It.Is<UInt16>(universe => universe == expectedUniverse)
				) == expectedIP

			);
			SacnSender sender = new SacnSender(udpWrapper, addressProvider);
			await sender.MulticastSendIPV4(expectedUniverse, packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedIP && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}

		[Fact]
		public async Task When_MulticastSendIPV6IsCalled_Then_ThePacketIsSent()
		{
			var expectedUniverse = (UInt16)1234;
			var packet = new DataPacket();
			packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };
			packet.FramingLayer.SourceName = "Source";
			byte[] expectedBytes = new byte[packet.Length];
			packet.Write(expectedBytes);

			var expectedIP = IPAddress.Parse("127.0.0.1");

			var udpWrapper = Mock.Of<IUdpWrapper>();
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetIPV6MulticastAddress(
					It.Is<UInt16>(universe => universe == expectedUniverse)
				) == expectedIP

			);
			SacnSender sender = new SacnSender(udpWrapper, addressProvider);
			await sender.MulticastSendIPV6(expectedUniverse, packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedIP && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}
	}
}
