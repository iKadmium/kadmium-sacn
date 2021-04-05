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
	public class DataPacketFramingLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			var expectedVector = FramingLayerVector.VECTOR_E131_DATA_PACKET;
			byte expectedPriority = 27;
			UInt16 expectedSyncAddress = 25;
			byte expectedSequenceNumber = 127;
			bool previewData = true;
			bool streamTerminated = true;
			bool forceSynchronization = true;
			byte expectedOptions = 0b0000_1110;
			UInt16 universe = 258;
			string expectedSourceName = "This is the source name";

			var bytes = GetDataPacketFramingLayer(expectedSourceName, expectedPriority, expectedSyncAddress, expectedSequenceNumber, expectedOptions, universe);

			var framingLayer = DataPacketFramingLayer.Parse(bytes.ToArray());
			Assert.Equal(expectedVector, framingLayer.Vector);
			Assert.Equal(expectedPriority, framingLayer.Priority);
			Assert.Equal(expectedSyncAddress, framingLayer.SynchronizationAddress);
			Assert.Equal(expectedSequenceNumber, framingLayer.SequenceNumber);
			Assert.Equal(previewData, framingLayer.PreviewData);
			Assert.Equal(streamTerminated, framingLayer.StreamTerminated);
			Assert.Equal(forceSynchronization, framingLayer.ForceSynchronization);
			Assert.Equal(expectedOptions, framingLayer.Options);
			Assert.Equal(universe, framingLayer.Universe);
		}

		[Theory]
		[InlineData(true, true, true, 0b0000_1110)]
		[InlineData(true, true, false, 0b0000_1100)]
		[InlineData(true, false, true, 0b0000_1010)]
		[InlineData(true, false, false, 0b0000_1000)]
		[InlineData(false, true, true, 0b0000_0110)]
		[InlineData(false, true, false, 0b0000_0100)]
		[InlineData(false, false, true, 0b0000_0010)]
		[InlineData(false, false, false, 0b0000_0000)]
		public void When_TheOptionsFlagsAreSet_Then_TheResultsAreAsExpected(bool forceSynchronization, bool streamTerminated, bool previewData, byte expectedOptions)
		{
			DataPacketFramingLayer framingLayer = new DataPacketFramingLayer();
			framingLayer.ForceSynchronization = forceSynchronization;
			framingLayer.StreamTerminated = streamTerminated;
			framingLayer.PreviewData = previewData;

			Assert.Equal(expectedOptions, framingLayer.Options);
		}

		[Fact]
		public void When_WriteIsCalled_Then_TheDataIsCorrect()
		{
			string sourceName = "My source name";
			byte priority = 134;
			UInt16 syncAddress = 1023;
			byte sequenceNumber = 135;
			byte options = 0;
			UInt16 universe = 1953;

			var framingLayer = new DataPacketFramingLayer()
			{
				SourceName = sourceName,
				Priority = priority,
				SynchronizationAddress = syncAddress,
				SequenceNumber = sequenceNumber,
				Options = options,
				Universe = universe
			};

			var expectedBytes = GetDataPacketFramingLayer(sourceName, priority, syncAddress, sequenceNumber, options, universe);

			using var owner = MemoryPool<byte>.Shared.Rent(DataPacketFramingLayer.Length);
			var actualBytes = owner.Memory.Span.Slice(0, DataPacketFramingLayer.Length);
			framingLayer.Write(actualBytes, 0);
			Assert.Equal(expectedBytes.ToArray(), actualBytes.ToArray());
		}

		[Fact]
		public void When_TheUniverseIsTooHigh_Then_AnExceptionIsThrown()
		{
			var framingLayer = new DataPacketFramingLayer();
			Assert.Throws<ArgumentOutOfRangeException>(() => framingLayer.Universe = DataPacketFramingLayer.Universe_MaxValue + 1);
		}

		[Fact]
		public void When_TheUniverseIsTooLow_Then_AnExceptionIsThrown()
		{
			var framingLayer = new DataPacketFramingLayer();
			Assert.Throws<ArgumentOutOfRangeException>(() => framingLayer.Universe = DataPacketFramingLayer.Universe_MinValue - 1);
		}

		[Fact]
		public void When_ThePriorityIsTooHigh_Then_AnExceptionIsThrown()
		{
			var framingLayer = new DataPacketFramingLayer();
			Assert.Throws<ArgumentOutOfRangeException>(() => framingLayer.Priority = DataPacketFramingLayer.Priority_MaxValue + 1);
		}

		public static List<byte> GetDataPacketFramingLayer(string sourceName, byte priority, UInt16 syncAddress, byte sequenceNumber, byte options, UInt16 universe)
		{
			var bytes = new List<byte>(new byte[] {
				(0x7 << 4), DataPacketFramingLayer.Length, // flags and length
				0x00, 0x00, 0x00, (byte)FramingLayerVector.VECTOR_E131_DATA_PACKET,
			});

			bytes.AddRange(Encoding.UTF8.GetBytes(sourceName));
			bytes.AddRange(Enumerable.Repeat((byte)0, 64 - sourceName.Length));
			bytes.Add(priority);
			bytes.AddRange(new byte[] { (byte)((syncAddress & 0xFF00) >> 8), (byte)(syncAddress & 0xFF) });
			bytes.Add(sequenceNumber);
			bytes.Add(options);
			bytes.AddRange(new byte[] { (byte)((universe & 0xFF00) >> 8), (byte)(universe & 0xFF) });

			return bytes;
		}

	}
}
