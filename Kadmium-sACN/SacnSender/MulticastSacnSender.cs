using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kadmium_sACN.SacnSender
{
	public abstract class MulticastSacnSender : SacnSender
	{
		private ISacnMulticastAddressProvider MulticastAddressProvider { get; }
		
		protected MulticastSacnSender(IUdpWrapper udpWrapper, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpWrapper)
		{
			MulticastAddressProvider = multicastAddressProvider;
		}
		
		public Task Send(DataPacket packet)
		{
			return SendInternal(MulticastAddressProvider.GetMulticastAddress(packet.FramingLayer.Universe), packet);
		}

		public Task Send(UniverseDiscoveryPacket packet)
		{
			return SendInternal(MulticastAddressProvider.GetMulticastAddress(UniverseDiscoveryPacket.DiscoveryUniverse), packet);
		}

		public Task Send(SynchronizationPacket packet)
		{
			return SendInternal(MulticastAddressProvider.GetMulticastAddress(packet.FramingLayer.SynchronizationAddress), packet);
		}
	}
}
