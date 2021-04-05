using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Kadmium_sACN.SacnSender
{
	public abstract class SacnSender : IDisposable
	{
		private IUdpWrapper UdpWrapper { get; }
		private ISacnMulticastAddressProvider MulticastAddressProvider { get; }

		protected SacnSender(IUdpWrapper udpWrapper)
		{
			UdpWrapper = udpWrapper;
		}

		public SacnSender() : this(new UdpWrapper()) { }

		protected async Task SendInternal(IPAddress address, SacnPacket packet)
		{
			using (var owner = MemoryPool<byte>.Shared.Rent(packet.Length))
			{
				var bytes = owner.Memory.Slice(0, packet.Length);
				packet.Write(bytes.Span);
				await UdpWrapper.Send(new IPEndPoint(address, Constants.Port), bytes);
			}
		}

		public void Dispose()
		{
			UdpWrapper?.Dispose();
		}
	}
}
