using Kadmium_sACN.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SACNLayerTests
	{
		private class TestSACNLayer : SACNLayer
		{
			public override int Length => throw new NotImplementedException();
		}

		[Theory]
		[InlineData(0b1111_1111_1111_1111, 0b0000_1111, 0b0000_1111_1111_1111)]
		[InlineData(0b0000_0000_0000_0000, 0b0000_0000, 0b0000_0000_0000_0000)]
		[InlineData(0b0101_0101_0101_0101, 0b0000_0101, 0b0000_0101_0101_0101)]
		public void When_FlagsAndLengthIsSet_Then_TheLengthAndFlagsFieldsAreAsExpected(UInt16 flagsAndLength, byte expectedFlags, UInt16 expectedLength)
		{
			TestSACNLayer layer = new TestSACNLayer();
			layer.FlagsAndLength = flagsAndLength;
			Assert.Equal(expectedFlags, layer.Flags);
			Assert.Equal(expectedLength, layer.PDULength);
		}

		[Theory]
		[InlineData(0b0000_1111, 0b0000_1111_1111_1111, 0b1111_1111_1111_1111)]
		[InlineData(0b0000_0000, 0b0000_0000_0000_0000, 0b0000_0000_0000_0000)]
		[InlineData(0b0000_0101, 0b0000_0101_0101_0101, 0b0101_0101_0101_0101)]
		public void When_FlagsAndPDULengthAreSet_Then_TheFlagsAndLengthFieldIsAsExpected(byte flags, UInt16 length, UInt16 expectedFlagsAndLength)
		{
			TestSACNLayer layer = new TestSACNLayer();
			layer.Flags = flags;
			layer.PDULength = length;
			Assert.Equal(expectedFlagsAndLength, layer.FlagsAndLength);
		}
	}
}
