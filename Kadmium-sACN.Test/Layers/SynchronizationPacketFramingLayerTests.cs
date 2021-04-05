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
	public class SynchronizationPacketFramingLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			UInt16 synchronizationAddress = 1028;
			byte sequenceNumber = 67;
			var bytes = GetSynchronizationPacketFramingLayer(sequenceNumber, synchronizationAddress);
			
			var framingLayer = SynchronizationPacketFramingLayer.Parse(bytes.ToArray());
			Assert.Equal(FramingLayerVector.VECTOR_E131_EXTENDED_SYNCHRONIZATION, framingLayer.Vector);
			Assert.Equal(sequenceNumber, framingLayer.SequenceNumber);
			Assert.Equal(synchronizationAddress, framingLayer.SynchronizationAddress);
		}

		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			UInt16 syncAddress = 1023;
			byte sequenceNumber = 135;
			
			var framingLayer = new SynchronizationPacketFramingLayer()
			{
				SynchronizationAddress = syncAddress,
				SequenceNumber = sequenceNumber
			};

			var expectedBytes = GetSynchronizationPacketFramingLayer(sequenceNumber, syncAddress);

			using var owner = MemoryPool<byte>.Shared.Rent(SynchronizationPacketFramingLayer.Length);
			var actualBytes = owner.Memory.Span.Slice(0, SynchronizationPacketFramingLayer.Length);
			framingLayer.Write(actualBytes);
			Assert.Equal(expectedBytes.ToArray(), actualBytes.ToArray());
		}

		public static List<byte> GetSynchronizationPacketFramingLayer(byte sequenceNumber, UInt16 synchronizationAddress)
		{
			List<byte> bytes = new List<byte>();
			bytes.AddRange(new byte[] { 0x7 << 4, 11 }); // flags and length
			bytes.AddRange(new byte[] { 0, 0, 0, (byte)FramingLayerVector.VECTOR_E131_EXTENDED_SYNCHRONIZATION }); // Vector
			bytes.Add(sequenceNumber);
			bytes.AddRange(new byte[] { (byte)((synchronizationAddress & 0xFF00) >> 8) , (byte)(synchronizationAddress & 0xFF) });
			bytes.AddRange(new byte[] { 0, 0 }); // reserved

			return bytes;
		}
	}
}
