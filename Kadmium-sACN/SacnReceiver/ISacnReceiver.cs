using System;
using System.Net;

namespace Kadmium_sACN.SacnReceiver
{
	public interface ISacnReceiver : IDisposable
	{
		event EventHandler<DataPacket> OnDataPacketReceived;
		event EventHandler<SynchronizationPacket> OnSynchronizationPacketReceived;
		event EventHandler<UniverseDiscoveryPacket> OnUniverseDiscoveryPacketReceived;
		void Listen(IPAddress address);
		IPEndPoint HostEndPoint { get; }
	}
}