using Kadmium_sACN.Layers;
using System;
using System.Buffers;
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
			var expectedCid = new byte[]
			{
				1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
			};
			var expectedVector = RootLayerVector.VECTOR_ROOT_E131_DATA;
			var bytes = GetRootLayer(expectedCid, expectedVector);

			var rootLayer = RootLayer.Parse(bytes.ToArray());

			Assert.Equal(expectedVector, rootLayer.Vector);
			Assert.Equal(expectedCid, rootLayer.CID);
		}

		[Fact]
		public void Given_ThePreambleSizeIsWrong_When_ParseIsCalled_Then_NullIsReturned()
		{
			var expectedVector = RootLayerVector.VECTOR_ROOT_E131_DATA;
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

			Assert.Null(RootLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void Given_ThePostambleSizeIsWrong_When_ParseIsCalled_Then_NullIsReturned()
		{
			var expectedVector = RootLayerVector.VECTOR_ROOT_E131_DATA;
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

			Assert.Null(RootLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void Given_TheSequenceIdentifierIsWrong_When_ParseIsCalled_Then_NullIsReturned()
		{
			var expectedVector = RootLayerVector.VECTOR_ROOT_E131_DATA;
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

			Assert.Null(RootLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			var expectedCid = new byte[]
			{
				1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16
			};
			var expectedVector = RootLayerVector.VECTOR_ROOT_E131_DATA;
			var expectedBytes = GetRootLayer(expectedCid, expectedVector);

			RootLayer rootLayer = new RootLayer()
			{
				CID = expectedCid,
				Vector = RootLayerVector.VECTOR_ROOT_E131_DATA
			};

			using var owner = MemoryPool<byte>.Shared.Rent(RootLayer.LENGTH);
			var actualBytes = owner.Memory.Span.Slice(0, RootLayer.LENGTH);
			rootLayer.Write(actualBytes, 0);
			Assert.Equal(expectedBytes, actualBytes.ToArray());
		}

		public static List<byte> GetRootLayer(byte[] cid, RootLayerVector vector)
		{
			var expectedVector = vector;
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

			bytes.AddRange(cid);

			return bytes;
		}
	}
}
