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

		[Fact]
		public void Given_ThePreambleSizeIsWrong_When_ParseIsCalled_Then_AnExceptionIsThrown()
		{
			var expectedVector = RootLayer.VECTOR_ROOT_E131_DATA;
			var expectedFlags = RootLayer.FLAGS;
			var expectedLength = RootLayer.LENGTH - 16;

			var bytes = new List<byte>
			{
				0x00, 0x99, // preamble size
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

			Assert.Throws<ArgumentException>(() => RootLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void Given_ThePostambleSizeIsWrong_When_ParseIsCalled_Then_AnExceptionIsThrown()
		{
			var expectedVector = RootLayer.VECTOR_ROOT_E131_DATA;
			var expectedFlags = RootLayer.FLAGS;
			var expectedLength = RootLayer.LENGTH - 16;

			var bytes = new List<byte>
			{
				0x00, 0x10, // preamble size
				0x00, 0x99, // postamble size
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

			Assert.Throws<ArgumentException>(() => RootLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void Given_TheSequenceIdentifierIsWrong_When_ParseIsCalled_Then_AnExceptionIsThrown()
		{
			var expectedVector = RootLayer.VECTOR_ROOT_E131_DATA;
			var expectedFlags = RootLayer.FLAGS;
			var expectedLength = RootLayer.LENGTH - 16;

			var bytes = new List<byte>
			{
				0x00, 0x10, // preamble size
				0x00, 0x00, // postamble size
			};
			bytes.AddRange(new byte[]
			{
				0x41, 0x53, 0x43, 0x2d, 0x45, 0x31, 0x2e, 0x31, 0x37, 0x00, 0x00, 0x32
			});
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

			Assert.Throws<ArgumentException>(() => RootLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void Given_TheFlagsAreWrong_When_ParseIsCalled_Then_AnExceptionIsThrown()
		{
			var expectedVector = RootLayer.VECTOR_ROOT_E131_DATA;
			var expectedFlags = 0x1;
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

			Assert.Throws<ArgumentException>(() => RootLayer.Parse(bytes.ToArray()));
		}
	}
}
