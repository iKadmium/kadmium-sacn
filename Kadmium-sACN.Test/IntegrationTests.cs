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
	public class IntegrationTests
	{
		[Fact]
		public async Task When_ListeningAndSendingUnicastWithIPV4_Then_MessagesAreSentAndReceived()
		{
			using (SacnSender sender = new SacnSender())
			{
				using (SacnReceiver receiver = new SacnReceiver())
				{
					DataPacket received = null;

					var hostname = Dns.GetHostName();
					var ipAddresses = await Dns.GetHostAddressesAsync(hostname);
					var address = ipAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(address);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = 25;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.UnicastSend(address, packet);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}

		[Fact]
		public async Task When_ListeningAndSendingUnicastWithIPV6_Then_MessagesAreSentAndReceived()
		{
			using (SacnSender sender = new SacnSender())
			{
				using (SacnReceiver receiver = new SacnReceiver())
				{
					DataPacket received = null;

					var hostname = Dns.GetHostName();
					var ipAddresses = await Dns.GetHostAddressesAsync(hostname);
					var address = ipAddresses.First(x => x.AddressFamily == AddressFamily.InterNetworkV6);

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(address);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = 25;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.UnicastSend(address, packet);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}

		[Fact]
		public async Task When_ListeningAndSendingMulticastOnIPV4_Then_MessagesAreSentAndReceived()
		{
			UInt16 universe = 1;
			using (SacnSender sender = new SacnSender())
			{
				using (SacnReceiver receiver = new SacnReceiver())
				{
					DataPacket received = null;

					var hostname = Dns.GetHostName();
					var ipAddresses = await Dns.GetHostAddressesAsync(hostname);

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(IPAddress.Any);
					receiver.JoinMulticastGroup(universe);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = universe;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.MulticastSendIPV4(universe, packet);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}

		[Fact]
		public async Task When_ListeningAndSendingMulticastOnIPV6_Then_MessagesAreSentAndReceived()
		{
			UInt16 universe = 1;
			using (SacnSender sender = new SacnSender())
			{
				using (SacnReceiver receiver = new SacnReceiver())
				{
					DataPacket received = null;

					var hostname = Dns.GetHostName();
					var ipAddresses = await Dns.GetHostAddressesAsync(hostname);

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(IPAddress.IPv6Any);
					receiver.JoinMulticastGroup(universe);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = universe;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.MulticastSendIPV6(universe, packet);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}
	}
}
