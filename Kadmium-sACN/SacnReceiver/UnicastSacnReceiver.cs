using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kadmium_sACN.SacnReceiver
{
	public class UnicastSacnReceiver : SacnReceiver
	{
		public UnicastSacnReceiver() : base(new UdpWrapper())
		{

		}

		public override void Listen(IPAddress address)
		{
			ListenInternal(address);
		}
	}
}
