using Kadmium_sACN.SacnReceiver;
using Kadmium_sACN.SacnSender;
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
			var hostname = Dns.GetHostName();
			var ipAddresses = await Dns.GetHostAddressesAsync(hostname);
			var address = ipAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);

			using (var sender = new Kadmium_sACN.SacnSender.SacnSender())
			{
				using (var receiver = new Kadmium_sACN.SacnReceiver.SacnReceiver())
				{
					DataPacket received = null;

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(address);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = 25;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.SendUnicast(packet, address);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}

		[Fact]
		public async Task When_ListeningAndSendingUnicastWithIPV6_Then_MessagesAreSentAndReceived()
		{
			var hostname = Dns.GetHostName();
			var ipAddresses = await Dns.GetHostAddressesAsync(hostname);
			var address = ipAddresses.First(x => x.AddressFamily == AddressFamily.InterNetworkV6);

			using (var sender = new Kadmium_sACN.SacnSender.SacnSender())
			{
				using (var receiver = new Kadmium_sACN.SacnReceiver.SacnReceiver())
				{
					DataPacket received = null;

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(address);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = 25;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.SendUnicast(packet, address);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}

		// [Fact]
		// public async Task When_ListeningAndSendingMulticastOnIPV4_Then_MessagesAreSentAndReceived()
		// {
		// 	UInt16 universe = 1;
		// 	using (var sender = new SacnSender.SacnSender())
		// 	{
		// 		using (var receiver = new SacnReceiver.SacnReceiver())
		// 		{
		// 			DataPacket received = null;

		// 			receiver.OnDataPacketReceived += (sender, packet) =>
		// 			{
		// 				received = packet;
		// 			};
		// 			receiver.Listen(IPAddress.Any);
		// 			await Task.Delay(250);
		// 			receiver.JoinMulticastGroup(universe, false);

		// 			var packet = new DataPacket();
		// 			packet.FramingLayer.SourceName = "Source name";
		// 			packet.FramingLayer.Universe = universe;
		// 			packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

		// 			await sender.SendMulticast(packet, false);
		// 			await Task.Delay(250);
		// 			Assert.NotNull(received);
		// 		}
		// 	}
		// }

		[Fact]
		public async Task When_ListeningAndSendingMulticastOnIPV6_Then_MessagesAreSentAndReceived()
		{
			UInt16 universe = 1;
			using (var sender = new SacnSender.SacnSender())
			{
				using (var receiver = new SacnReceiver.SacnReceiver())
				{
					DataPacket received = null;

					receiver.OnDataPacketReceived += (sender, packet) =>
					{
						received = packet;
					};
					receiver.Listen(IPAddress.IPv6Any);
					await Task.Delay(250);
					receiver.JoinMulticastGroup(universe, true);

					var packet = new DataPacket();
					packet.FramingLayer.SourceName = "Source name";
					packet.FramingLayer.Universe = universe;
					packet.DMPLayer.PropertyValues = new byte[] { 1, 2, 3, 4 };

					await sender.SendMulticast(packet, true);
					await Task.Delay(250);
					Assert.NotNull(received);
				}
			}
		}
	}
}
