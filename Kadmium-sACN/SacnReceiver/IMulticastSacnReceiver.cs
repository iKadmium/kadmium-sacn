using System;
using System.Collections.Generic;

namespace Kadmium_sACN.SacnReceiver
{
	public interface IMulticastSacnReceiver : ISacnReceiver
	{
		void JoinMulticastGroups(IEnumerable<UInt16> universes, bool ipv6);
		void JoinMulticastGroup(UInt16 universe, bool ipv6);
		void DropMulticastGroups(IEnumerable<UInt16> universes, bool ipv6);
		void DropMulticastGroup(UInt16 universe, bool ipv6);
	}
}