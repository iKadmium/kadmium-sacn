using Kadmium_sACN.MulticastAddressProvider;
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
	public class MulticastSacnSenderTests
	{
		private class TestMulticastSacnSender : MulticastSacnSender
		{
			public TestMulticastSacnSender(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpWrapper, multicastAddressProvider)
			{
			}
		}

		[Fact]
		public async Task Given_ThePacketIsADataPacket_When_MulticastSendIsCalled_Then_ThePacketIsSent()
		{
			var expectedUniverse = (UInt16)1234;
			var packet = new DataPacket();
			packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };
			packet.FramingLayer.SourceName = "Source";
			packet.FramingLayer.Universe = expectedUniverse;
			byte[] expectedBytes = new byte[packet.Length];
			packet.Write(expectedBytes);

			var expectedIP = IPAddress.Parse("127.0.0.1");

			var udpWrapper = Mock.Of<IUdpWrapper>();
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetMulticastAddress(
					It.Is<UInt16>(universe => universe == expectedUniverse)
				) == expectedIP

			);
			var sender = new TestMulticastSacnSender(udpWrapper, addressProvider);
			await sender.Send(packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedIP && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}

		[Fact]
		public async Task Given_ThePacketIsAUniverseDiscoveryPacket_When_MulticastSendIPV4IsCalled_Then_ThePacketIsSent()
		{
			var expectedUniverse = UniverseDiscoveryPacket.DiscoveryUniverse;
			var packet = new UniverseDiscoveryPacket();
			packet.UniverseDiscoveryLayer.Universes = new UInt16[] { 1 };
			packet.FramingLayer.SourceName = "Source";
			byte[] expectedBytes = new byte[packet.Length];
			packet.Write(expectedBytes);

			var expectedIP = IPAddress.Parse("127.0.0.1");

			var udpWrapper = Mock.Of<IUdpWrapper>();
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetMulticastAddress(
					It.Is<UInt16>(universe => universe == expectedUniverse)
				) == expectedIP

			);
			var sender = new TestMulticastSacnSender(udpWrapper, addressProvider);
			await sender.Send(packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedIP && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}

		[Fact]
		public async Task Given_ThePacketIsASynchronizationPacket_When_MulticastSendIPV4IsCalled_Then_ThePacketIsSent()
		{
			var expectedUniverse = (UInt16)1234;
			var packet = new SynchronizationPacket();
			packet.FramingLayer.SynchronizationAddress = expectedUniverse;
			byte[] expectedBytes = new byte[packet.Length];
			packet.Write(expectedBytes);

			var expectedIP = IPAddress.Parse("127.0.0.1");

			var udpWrapper = Mock.Of<IUdpWrapper>();
			var addressProvider = Mock.Of<ISacnMulticastAddressProvider>(x =>
				x.GetMulticastAddress(
					It.Is<UInt16>(universe => universe == expectedUniverse)
				) == expectedIP

			);
			var sender = new TestMulticastSacnSender(udpWrapper, addressProvider);
			await sender.Send(packet);

			Mock.Get(udpWrapper)
				.Verify(x => x.Send(
					It.Is<IPEndPoint>(x => x.Address == expectedIP && x.Port == 5568),
					It.Is<ReadOnlyMemory<byte>>(x => x.ToArray().SequenceEqual(expectedBytes))
				));
		}
	}
}
