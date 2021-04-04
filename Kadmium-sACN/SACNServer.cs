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
	public class SACNServer : IDisposable
	{
		private const int Port = 5568;
		public const int Universe_MinValue = 1;
		public const int Universe_MaxValue = 63999;

		private IUdpWrapper UdpWrapper { get; }
		public event EventHandler<DataPacket> OnDataPacketReceived;
		public event EventHandler<SynchronizationPacket> OnSynchronizationPacketReceived;
		public event EventHandler<UniverseDiscoveryPacket> OnUniverseDiscoveryPacketReceived;

		public IPEndPoint HostEndPoint => UdpWrapper.HostEndPoint;
		
		internal SACNServer(IUdpWrapper udpWrapper)
		{
			UdpWrapper = udpWrapper;
		}

		public SACNServer() : this(new UdpWrapper()) { }

		public void Listen()
		{
			RegisterListeners();
			UdpWrapper.Listen(Port);
		}

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

			byte[] universeBytes = new byte[2];
			BinaryPrimitives.WriteUInt16BigEndian(universeBytes, universe);
			switch (UdpWrapper.HostEndPoint.AddressFamily)
			{
				case AddressFamily.InterNetwork:
					UdpWrapper.JoinMulticastGroup(GetIPV4MulticastAddress(universeBytes));
					break;
				case AddressFamily.InterNetworkV6:
					UdpWrapper.JoinMulticastGroup(GetIPV6MulticastAddress(universeBytes));
					break;
			}
		}

		private IPAddress GetIPV4MulticastAddress(Span<byte> universeBytes)
		{	
			IPAddress address = new IPAddress(new byte[] { 239, 255, universeBytes[0], universeBytes[1] });
			return address;
		}

		private IPAddress GetIPV6MulticastAddress(Span<byte> universeBytes)
		{
			string ipString = $"FF18::83:00:{universeBytes[0].ToString("X2")}:{universeBytes[1].ToString("X2")}";
			IPAddress address = IPAddress.Parse(ipString);
			return address;
		}

		private void RegisterListeners()
		{
			UdpWrapper.OnPacketReceived += (object sender, UdpReceiveResult e) =>
			{
				var packet = SACNPacket.Parse(e.Buffer);
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
