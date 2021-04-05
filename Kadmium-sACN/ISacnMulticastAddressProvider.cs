using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kadmium_sACN
{
	public interface ISacnMulticastAddressProvider
	{
		IPAddress GetIPV4MulticastAddress(UInt16 universe);
		IPAddress GetIPV6MulticastAddress(UInt16 universe);
	}
}
