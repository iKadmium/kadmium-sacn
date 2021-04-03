using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SynchronizationPacketFramingLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			List<byte> bytes = new List<byte>();
			bytes.AddRange(new byte[] { 0x7 << 4, 11 }); // flags and length
			FramingLayerVector expectedVector = FramingLayerVector.VECTOR_E131_EXTENDED_SYNCHRONIZATION;
			bytes.AddRange(new byte[] { 0, 0, 0, (byte)expectedVector }); // Vector

			byte expectedSequenceNumber = 67;
			bytes.Add(expectedSequenceNumber);
			UInt16 synchronizationAddress = 1028;
			bytes.AddRange(new byte[] { 0x04, 0x04 });
			bytes.AddRange(new byte[] { 0, 0}); // reserved

			var framingLayer = SynchronizationPacketFramingLayer.Parse(bytes.ToArray());
			Assert.Equal(expectedVector, framingLayer.Vector);
			Assert.Equal(expectedSequenceNumber, framingLayer.SequenceNumber);
			Assert.Equal(synchronizationAddress, framingLayer.SynchronizationAddress);
		}
	}
}
