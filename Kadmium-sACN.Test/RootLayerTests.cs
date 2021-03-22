using Kadmium_sACN.Layers;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class RootLayerTests
	{
		[Fact]
		public void Given_TheDataIsValid_When_ParseIsCalled_Then_TheLayerParsesCorrectly()
		{
			var expectedVector = RootLayer.VECTOR_ROOT_E131_DATA;
			var expectedFlags = RootLayer.FLAGS;
			var expectedLength = RootLayer.LENGTH - 16;

			var bytes = new List<byte>
			{
				0x00, 0x10, // preamble size
				0x00, 0x00, // postamble size
			};
			bytes.AddRange(RootLayer.ACNIdentifier);
			bytes.AddRange(new byte[]
			{
				(byte)(expectedFlags << 4), (byte)expectedLength, // flags and length
				0x00, 0x00, 0x00, (byte)expectedVector, // vector
			});

			var expectedCid = new List<byte>
			{
				1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
			};
			bytes.AddRange(expectedCid);

			var rootLayer = RootLayer.Parse(bytes.ToArray());

			Assert.Equal(expectedVector, rootLayer.Vector);
			Assert.Equal(expectedFlags, rootLayer.Flags);
			Assert.Equal(expectedLength, rootLayer.PDULength);
			Assert.Equal(expectedCid.ToArray(), rootLayer.CID);
		}
	}
}
