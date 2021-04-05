using Kadmium_sACN;
using Kadmium_sACN.SacnReceiver;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SacnViewer
{
	class Program
	{
		static void Main(string[] args)
		{
			using var receiver = new MulticastSacnReceiverIPV4();
			receiver.OnDataPacketReceived += (sender, packet) =>
			{
				Console.WriteLine(packet.FramingLayer.Universe + ": ");
				Console.WriteLine(string.Join(", ", packet.DMPLayer.PropertyValues));
			};
			receiver.OnSynchronizationPacketReceived += (sender, packet) =>
			{
				Console.WriteLine("Sync!");
			};
			receiver.OnUniverseDiscoveryPacketReceived += (sender, packet) =>
			{
				Console.WriteLine("Discovery!");
			};
			receiver.Listen(IPAddress.Any);
			receiver.JoinMulticastGroup(1);
			Console.WriteLine("Listening. Press enter to exit");
			Console.ReadLine();
		}
	}
}
