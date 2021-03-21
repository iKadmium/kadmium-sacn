using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public abstract class SACNLayer
	{
		public static UInt16 LowTwelveBits = 0b0000_1111_1111_1111;
		public static UInt16 HighFourBits = 0b1111_0000_0000_0000;
		
		public UInt16 FlagsAndLength
		{
			get
			{
				return (UInt16)((UInt16)(PDULength & LowTwelveBits) | (UInt16)((Flags << 12) & HighFourBits));
			}
			set
			{
				PDULength = (UInt16)(value & LowTwelveBits);
			}
		}

		public byte Flags { get; } = 0x7;
		public UInt16 PDULength { get; set; }
		public abstract int Length { get; }
	}
}
