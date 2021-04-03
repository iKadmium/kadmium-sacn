using Kadmium_sACN.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class UniverseDiscoveryLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			List<byte> bytes = new List<byte>();
			int universeCount = 5;
			byte length = (byte)(UniverseDiscoveryLayer.MIN_LENGTH + (universeCount << 1));
			bytes.AddRange(new byte[] { 0x7 << 4, (length) }); // flags and length
			UInt32 expectedVector = UniverseDiscoveryLayer.VECTOR_UNIVERSE_DISCOVERY_UNIVERSE_LIST;
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

			var framingLayer = UniverseDiscoveryLayer.Parse(bytes.ToArray());
			Assert.Equal(expectedVector, framingLayer.Vector);
			Assert.Equal(page, framingLayer.Page);
			Assert.Equal(lastPage, framingLayer.LastPage);
			Assert.Equal(expectedUniverses, framingLayer.Universes);
		}

		[Fact]
		public void Given_TheDataContainsTheWrongVector_When_ParseIsCalled_Then_NullIsReturned()
		{
			List<byte> bytes = new List<byte>();
			int universeCount = 5;
			byte length = (byte)(UniverseDiscoveryLayer.MIN_LENGTH + (universeCount << 1));
			UInt16 expectedFlagsAndLength = (UInt16)(0x7 << 12 | length);
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
	}
}
