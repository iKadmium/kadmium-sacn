using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kadmium_sACN.SacnReceiver
{
	public class UnicastSacnReceiver : SacnReceiver
	{
		public UnicastSacnReceiver(AddressFamily addressFamily = AddressFamily.InterNetwork) : base(new UdpPipeline(addressFamily))
		{
		}

		public override void Listen(IPAddress address)
		{
			ListenInternal(address);
		}
	}
}
