using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.SacnSender
{
	public class MulticastSacnSenderIPV4 : MulticastSacnSender
	{
		public MulticastSacnSenderIPV4() : base(new UdpWrapper(), new SacnMulticastAddressProviderIPV4())
		{
		}
	}
}
