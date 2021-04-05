using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class IEnumerableExtensionsTest
	{
		[Fact]
		public void Given_TheCollectionIsSmallerThanTheChunkSize_When_ChunkIsCalled_Then_OneChunkIsReturned()
		{
			var collection = Enumerable.Range(1, 3);
			var chunks = collection.Chunk(10);
			Assert.Single(chunks);
		}

		[Fact]
		public void When_ChunkIsCalled_Then_TheChunkCountIsAsExpected()
		{
			var collection = Enumerable.Range(1, 10);
			var chunks = collection.Chunk(2);
			Assert.Equal(5, chunks.Count());
		}

		[Fact]
		public void When_ChunkIsCalled_Then_TheChunksContainTheExpectedValue()
		{
			var collection = Enumerable.Range(1, 10);
			var chunks = collection.Chunk(2);
			var expectedValues = new[]
			{
				new [] {1, 2},
				new [] {3, 4},
				new [] {5, 6},
				new [] {7, 8},
				new [] {9, 10},
			};
			Assert.Equal(expectedValues, chunks);
		}
	}
}
