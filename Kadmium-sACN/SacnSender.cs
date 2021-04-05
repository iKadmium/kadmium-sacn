using Kadmium_Udp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kadmium_sACN
{
	public class SacnSender : IDisposable
	{
		private const int Port = 5568;
		private IUdpWrapper UdpWrapper { get; }
		private ISacnMulticastAddressProvider MulticastAddressProvider { get; }

		internal SacnSender(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider)
		{
			UdpWrapper = udpWrapper;
			MulticastAddressProvider = multicastAddressProvider;
		}

		public SacnSender() : this(new UdpWrapper(), new SacnMulticastAddressProvider()) { }

		public async Task UnicastSend(IPAddress address, SacnPacket packet)
		{
			IPEndPoint endPoint = new IPEndPoint(address, Port);

			using (var owner = MemoryPool<byte>.Shared.Rent(packet.Length))
			{
				var bytes = owner.Memory.Slice(0, packet.Length);
				packet.Write(bytes.Span);
				await UdpWrapper.Send(endPoint, bytes);
			}
		}

		public Task MulticastSendIPV4(UInt16 universe, SacnPacket packet)
		{
			return UnicastSend(MulticastAddressProvider.GetIPV4MulticastAddress(universe), packet);
		}

		public Task MulticastSendIPV6(UInt16 universe, SacnPacket packet)
		{
			return UnicastSend(MulticastAddressProvider.GetIPV6MulticastAddress(universe), packet);
		}

		public void Dispose()
		{
			UdpWrapper?.Dispose();
		}
	}
}
