using Kadmium_Udp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kadmium_sACN.SacnReceiver
{
	public abstract class SacnReceiver : IDisposable
	{
		protected IUdpWrapper UdpWrapper { get; }
		
		public event EventHandler<DataPacket> OnDataPacketReceived;
		public event EventHandler<SynchronizationPacket> OnSynchronizationPacketReceived;
		public event EventHandler<UniverseDiscoveryPacket> OnUniverseDiscoveryPacketReceived;

		public IPEndPoint HostEndPoint => UdpWrapper.HostEndPoint;
		
		protected SacnReceiver(IUdpWrapper udpWrapper)
		{
			UdpWrapper = udpWrapper;
		}

		public abstract void Listen(IPAddress address);

		protected void ListenInternal(IPAddress address)
		{
			RegisterListeners();
			var endpoint = new IPEndPoint(address, Constants.Port);
			UdpWrapper.Listen(endpoint);
		}

		private void RegisterListeners()
		{
			UdpWrapper.OnPacketReceived += (object sender, UdpReceiveResult e) =>
			{
				var packet = SacnPacket.Parse(e.Buffer);
				switch (packet)
				{
					case DataPacket dataPacket:
						OnDataPacketReceived?.Invoke(this, dataPacket);
						break;
					case SynchronizationPacket syncPacket:
						OnSynchronizationPacketReceived?.Invoke(this, syncPacket);
						break;
					case UniverseDiscoveryPacket discoveryPacket:
						OnUniverseDiscoveryPacketReceived?.Invoke(this, discoveryPacket);
						break;
				}
			};
		}

		public void Dispose()
		{
			UdpWrapper?.Dispose();
		}
	}
}
