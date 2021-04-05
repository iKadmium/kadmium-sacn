using Kadmium_Udp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kadmium_sACN
{
	public class SacnReceiver : IDisposable
	{
		private const int Port = 5568;
		public const int Universe_MinValue = 1;
		public const int Universe_MaxValue = 63999;

		private IUdpWrapper UdpWrapper { get; }
		private ISacnMulticastAddressProvider MulticastAddressProvider { get; }

		public event EventHandler<DataPacket> OnDataPacketReceived;
		public event EventHandler<SynchronizationPacket> OnSynchronizationPacketReceived;
		public event EventHandler<UniverseDiscoveryPacket> OnUniverseDiscoveryPacketReceived;

		public IPEndPoint HostEndPoint => UdpWrapper.HostEndPoint;
		
		internal SacnReceiver(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider)
		{
			UdpWrapper = udpWrapper;
			MulticastAddressProvider = multicastAddressProvider;
		}

		public SacnReceiver() : this(new UdpWrapper(), new SacnMulticastAddressProvider()) { }

		public void Listen(IPAddress address)
		{
			RegisterListeners();
			var endpoint = new IPEndPoint(address, Port);
			UdpWrapper.Listen(endpoint);
		}

		public void JoinMulticastGroups(IEnumerable<UInt16> universes)
		{
			foreach (var universe in universes)
			{
				JoinMulticastGroup(universe);
			}
		}

		public void JoinMulticastGroup(UInt16 universe)
		{
			if (universe < Universe_MinValue || universe > Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Universe_MinValue} and {Universe_MaxValue} inclusive");
			}

			switch (UdpWrapper.HostEndPoint.AddressFamily)
			{
				case AddressFamily.InterNetwork:
					UdpWrapper.JoinMulticastGroup(MulticastAddressProvider.GetIPV4MulticastAddress(universe));
					break;
				case AddressFamily.InterNetworkV6:
					UdpWrapper.JoinMulticastGroup(MulticastAddressProvider.GetIPV6MulticastAddress(universe));
					break;
			}
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
