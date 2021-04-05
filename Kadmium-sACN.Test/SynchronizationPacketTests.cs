using Kadmium_sACN.Layers;
using Kadmium_sACN.Layers.Framing;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SynchronizationPacketTests
	{
		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			byte[] CID = Enumerable.Range(1, 12).Select(x => (byte)x).ToArray();
			byte sequenceNumber = 124;

			SynchronizationPacket packet = new SynchronizationPacket();
			packet.RootLayer.CID = CID;
			packet.FramingLayer.SequenceNumber = sequenceNumber;
			
			var expectedBytes = GetSynchronizationPacket(CID, sequenceNumber);

			using var owner = MemoryPool<byte>.Shared.Rent(packet.Length);
			var bytes = owner.Memory.Span.Slice(0, packet.Length);
			packet.Write(bytes);

			Assert.Equal(expectedBytes.ToArray(), bytes.ToArray());

		}

		public static Span<byte> GetSynchronizationPacket(byte[] CID, byte sequenceNumber)
		{
			var rootLayer = new RootLayer
			{
				CID = CID,
				Vector = RootLayerVector.VECTOR_ROOT_E131_EXTENDED
			};

			var framingLayer = new SynchronizationPacketFramingLayer
			{
				SequenceNumber = sequenceNumber
			};

			int totalLength = SynchronizationPacketFramingLayer.Length + RootLayer.Length;
			using var owner = MemoryPool<byte>.Shared.Rent(totalLength);
			var bytes = owner.Memory.Span.Slice(0, totalLength);

			rootLayer.Write(bytes, (UInt16)(SynchronizationPacketFramingLayer.Length));
			framingLayer.Write(bytes.Slice(RootLayer.Length));

			return bytes;
		}
	}
}
