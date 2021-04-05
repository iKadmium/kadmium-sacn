using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kadmium_sACN
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> collection, int chunkSize)
		{
			int count = collection.Count();
			int chunkCount = count / chunkSize;
			if (count % chunkSize > 0)
			{
				chunkCount++;
			}
			for (int i = 0; i < chunkCount; i++)
			{
				yield return collection.Skip(chunkSize * i).Take(chunkSize);
			}
		}
	}
}
