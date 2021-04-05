using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.Layers
{
	public class UniverseDiscoveryPacketFramingLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			string sourceName = "This is the source name";
			var bytes = GetUniverseDiscoveryPacketFramingLayer(sourceName);

			var framingLayer = UniverseDiscoveryPacketFramingLayer.Parse(bytes.ToArray());
			Assert.Equal(FramingLayerVector.VECTOR_E131_EXTENDED_DISCOVERY, framingLayer.Vector);
			Assert.Equal(sourceName, framingLayer.SourceName);
		}

		[Fact]
		public void Given_TheVectorIsIncorrect_When_ParseIsCalled_Then_NullIsReturned()
		{
			List<byte> bytes = new List<byte>();
			bytes.AddRange(new byte[] { 0x7 << 4, UniverseDiscoveryPacketFramingLayer.Length }); // flags and length
			bytes.AddRange(new byte[] { 0, 0, 0, (byte)42}); // Vector

			string sourceName = "This is the source name";
			bytes.AddRange(Encoding.UTF8.GetBytes(sourceName));
			bytes.AddRange(Enumerable.Repeat((byte)0, 64 - sourceName.Length));
			bytes.AddRange(new byte[] { 0, 0, 0, 0 }); // reserved

			Assert.Null(UniverseDiscoveryPacketFramingLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			string sourceName = "My source name";
			
			var framingLayer = new UniverseDiscoveryPacketFramingLayer()
			{
				SourceName = sourceName
			};

			var expectedBytes = GetUniverseDiscoveryPacketFramingLayer(sourceName);

			using var owner = MemoryPool<byte>.Shared.Rent(UniverseDiscoveryPacketFramingLayer.Length);
			var actualBytes = owner.Memory.Span.Slice(0, UniverseDiscoveryPacketFramingLayer.Length);
			framingLayer.Write(actualBytes, 0);
			Assert.Equal(expectedBytes.ToArray(), actualBytes.ToArray());
		}

		public static List<byte> GetUniverseDiscoveryPacketFramingLayer(string sourceName)
		{
			List<byte> bytes = new List<byte>();
			bytes.AddRange(new byte[] { 0x7 << 4, UniverseDiscoveryPacketFramingLayer.Length }); // flags and length
			FramingLayerVector expectedVector = FramingLayerVector.VECTOR_E131_EXTENDED_DISCOVERY;
			bytes.AddRange(new byte[] { 0, 0, 0, (byte)expectedVector }); // Vector

			bytes.AddRange(Encoding.UTF8.GetBytes(sourceName));
			bytes.AddRange(Enumerable.Repeat((byte)0, 64 - sourceName.Length));
			bytes.AddRange(new byte[] { 0, 0, 0, 0 }); // reserved

			return bytes;
		}
	}
}
