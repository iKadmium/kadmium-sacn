using Kadmium_sACN.Layers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.Layers
{
	public class UniverseDiscoveryLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			byte page = 27;
			byte lastPage = 28;
			
			var expectedUniverses = Enumerable.Range(byte.MaxValue, 1023)
				.Select(x => (UInt16)x)
				.ToArray();

			var bytes = GetUniverseDiscoveryLayer(expectedUniverses, page, lastPage);

			var universeDiscoveryLayer = UniverseDiscoveryLayer.Parse(bytes.ToArray());
			Assert.Equal(UniverseDiscoveryLayerVector.VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST, universeDiscoveryLayer.Vector);
			Assert.Equal(page, universeDiscoveryLayer.Page);
			Assert.Equal(lastPage, universeDiscoveryLayer.LastPage);
			Assert.Equal(expectedUniverses, universeDiscoveryLayer.Universes);
		}

		[Fact]
		public void Given_TheDataContainsTheWrongVector_When_ParseIsCalled_Then_NullIsReturned()
		{
			List<byte> bytes = new List<byte>();
			int universeCount = 5;
			byte length = (byte)(UniverseDiscoveryLayer.MinLength + (universeCount << 1));
			bytes.AddRange(new byte[] { 0x7 << 4, (length) }); // flags and length
			UInt32 expectedVector = 250;
			bytes.AddRange(new byte[] { 0, 0, 0, (byte)expectedVector }); // Vector

			byte page = 27;
			bytes.Add(page);
			byte lastPage = 28;
			bytes.Add(lastPage);

			var expectedUniverses = new UInt16[]
			{
				1,
				2,
				3,
				4,
				5
			};

			foreach (var expectedUniverse in expectedUniverses)
			{
				bytes.Add(0);
				bytes.Add((byte)expectedUniverse);
			}

			Assert.Null(UniverseDiscoveryLayer.Parse(bytes.ToArray()));
		}

		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			var universes = Enumerable.Range(byte.MaxValue, byte.MaxValue + 1023)
				.Select(x => (UInt16)x)
				.ToArray();
			byte page = 36;
			byte lastPage = 230;
			var expectedBytes = GetUniverseDiscoveryLayer(universes, page, lastPage);

			var universeDiscoveryLayer = new UniverseDiscoveryLayer()
			{
				Universes = universes,
				Page = page,
				LastPage = lastPage
			};

			using var owner = MemoryPool<byte>.Shared.Rent(UniverseDiscoveryLayer.MinLength + (universes.Length << 1));
			var actualBytes = owner.Memory.Span.Slice(0, UniverseDiscoveryLayer.MinLength + (universes.Length << 1));
			universeDiscoveryLayer.Write(actualBytes);
			Assert.Equal(expectedBytes, actualBytes.ToArray());
		}

		private static List<byte> GetUniverseDiscoveryLayer(IEnumerable<UInt16> universes, byte page, byte lastPage)
		{
			List<byte> bytes = new List<byte>();
			int universeCount = universes.Count();
			UInt16 length = (UInt16)(UniverseDiscoveryLayer.MinLength + (universeCount << 1));
			bytes.AddRange(new byte[] { (byte)((0x7 << 4) | (length & 0x0F00) >> 8), (byte)(length & 0xFF) }); // flags and length
			bytes.AddRange(new byte[] { 0, 0, 0, (byte)UniverseDiscoveryLayerVector.VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST }); // Vector

			bytes.Add(page);
			bytes.Add(lastPage);

			foreach(var universe in universes)
			{
				bytes.AddRange(BitConverter.IsLittleEndian ? BitConverter.GetBytes(universe).Reverse() : BitConverter.GetBytes(universe));
			}

			return bytes;
		}
	}
}
