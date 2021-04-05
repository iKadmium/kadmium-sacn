using Kadmium_Udp;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kadmium_sACN.SacnSender
{
	public class UnicastSacnSender : SacnSender
	{
		private IPAddress RemoteHost { get; }

		internal UnicastSacnSender(IUdpWrapper udpWrapper, IPAddress remoteHost) : base(udpWrapper)
		{
			RemoteHost = remoteHost;
		}

		public UnicastSacnSender(IPAddress remoteHost) : this(new UdpWrapper(), remoteHost)
		{ }

		public Task Send(DataPacket packet)
		{
			return SendInternal(RemoteHost, packet);
		}

		public Task Send(UniverseDiscoveryPacket packet)
		{
			return SendInternal(RemoteHost, packet);
		}

		public Task Send(SynchronizationPacket packet)
		{
			return SendInternal(RemoteHost, packet);
		}
	}
}
