using System;
using System.Collections.Generic;

namespace Kadmium_sACN.SacnReceiver
{
	public interface IMulticastSacnReceiver : ISacnReceiver
	{
		void JoinMulticastGroups(IEnumerable<UInt16> universes);
		void JoinMulticastGroup(UInt16 universe);
		void DropMulticastGroups(IEnumerable<UInt16> universes);
		void DropMulticastGroup(UInt16 universe);
	}
}