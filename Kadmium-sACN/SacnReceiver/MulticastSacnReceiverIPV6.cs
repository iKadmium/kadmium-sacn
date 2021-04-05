using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kadmium_sACN.SacnReceiver
{
	public class MulticastSacnReceiverIPV6 : MulticastSacnReceiver
	{
		internal MulticastSacnReceiverIPV6(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpWrapper, multicastAddressProvider)
		{
		}

		public MulticastSacnReceiverIPV6() : base(new UdpWrapper(), new SacnMulticastAddressProviderIPV6())
		{
		}

		public override void Listen(IPAddress ipAddress)
		{
			if (ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException("The given IP Address was not an IPv6 Address");
			}
			ListenInternal(ipAddress);
		}
	}
}
