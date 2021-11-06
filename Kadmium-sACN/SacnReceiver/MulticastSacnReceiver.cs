using Kadmium_sACN.Layers.Framing;
using Kadmium_sACN.MulticastAddressProvider;
using Kadmium_Udp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.SacnReceiver
{
	public abstract class MulticastSacnReceiver : SacnReceiver, IMulticastSacnReceiver
	{
		private ISacnMulticastAddressProvider MulticastAddressProvider { get; }

		protected MulticastSacnReceiver(IUdpPipeline udpPipeline, ISacnMulticastAddressProvider multicastAddressProvider) : base(udpPipeline)
		{
			MulticastAddressProvider = multicastAddressProvider;
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
			if (universe < Constants.Universe_MinValue || universe > Constants.Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Constants.Universe_MinValue} and {Constants.Universe_MaxValue} inclusive");
			}

			var multicastAddress = MulticastAddressProvider.GetMulticastAddress(universe);
			UdpPipeline.JoinMulticastGroup(multicastAddress);
		}

		public void DropMulticastGroups(IEnumerable<ushort> universes)
		{
			foreach (var universe in universes)
			{
				DropMulticastGroup(universe);
			}
		}

		public void DropMulticastGroup(ushort universe)
		{
			if (universe < Constants.Universe_MinValue || universe > Constants.Universe_MaxValue)
			{
				throw new ArgumentOutOfRangeException($"Universe must be between {Constants.Universe_MinValue} and {Constants.Universe_MaxValue} inclusive");
			}

			UdpPipeline.DropMulticastGroup(MulticastAddressProvider.GetMulticastAddress(universe));
		}
	}
}
