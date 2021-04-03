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

		public abstract int Length { get; }

		protected UInt16 GetFlagsAndLength(UInt16 Length)
		{
			return (UInt16)((FLAGS << 12) | Length);
		}
	}
}
