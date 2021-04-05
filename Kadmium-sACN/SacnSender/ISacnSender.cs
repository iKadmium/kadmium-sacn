using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kadmium_sACN.SacnSender
{
	interface ISacnSender : IDisposable
	{
		Task Send(DataPacket packet);
		Task Send(UniverseDiscoveryPacket packet);
		Task Send(SynchronizationPacket packet);
	}
}
