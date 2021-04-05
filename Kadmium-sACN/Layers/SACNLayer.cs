using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public abstract class SACNLayer
	{
		public const byte FLAGS = 0x7;
		public const UInt16 LengthMask = 0x0FFF;

		protected UInt16 GetFlagsAndLength(UInt16 length)
		{
			return (UInt16)((FLAGS << 12) | length);
		}
	}
}
