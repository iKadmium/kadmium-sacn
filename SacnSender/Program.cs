using Kadmium_sACN;
using Kadmium_sACN.SacnSender;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace SacnSenderTestUtil
{
	class Program
	{
		static async Task Main(string[] args)
		{
			byte[] cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			UInt16 universe = 1;
			string sourceName = "My lovely source";
			SacnPacketFactory factory = new SacnPacketFactory(cid, sourceName);
			using var sacnSender = new MulticastSacnSenderIPV4();

			using Timer timer = new Timer(10000);
			timer.Elapsed += async (sender, e) =>
			{
				var packets = factory.CreateUniverseDiscoveryPackets(new UInt16[] { universe });
				foreach (var packet in packets)
				{
					await sacnSender.Send(packet);
				}
			};
			timer.Start();

			byte[] values = new byte[512];
			while (true)
			{
				for (byte i = 0; i < 255; i++)
				{
					Array.Fill(values, i);
					var packet = factory.CreateDataPacket(universe, values);
					await sacnSender.Send(packet);
					await Task.Delay(1000 / 40);
				}
			}
		}
	}
}
