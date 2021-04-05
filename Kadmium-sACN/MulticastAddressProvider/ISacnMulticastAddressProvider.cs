using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Kadmium_sACN.MulticastAddressProvider
{
	public interface ISacnMulticastAddressProvider
	{
		IPAddress GetMulticastAddress(UInt16 universe);
	}
}
