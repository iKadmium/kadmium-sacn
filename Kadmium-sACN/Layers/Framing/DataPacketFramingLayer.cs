using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers.Framing
{
	public class DataPacketFramingLayer : FramingLayer
	{
		public const byte PreviewDataMask = 0b00000010;
		public const byte StreamTerminatedMask = 0b00000100;
		public const byte ForceSynchronizationMask = 0b00001000;

		public const int LENGTH = 77;

		public string SourceName { get; set; }
		public byte Priority { get; set; }
		public UInt16 SynchronizationAddress { get; set; }
		public byte SequenceNumber { get; set; }
		public bool PreviewData { get; set; }
		public bool StreamTerminated { get; set; }
		public bool ForceSynchronization { get; set; }
		public UInt16 Universe { get; set; }
		public override int Length => LENGTH;

		public byte Options
		{
			get
			{
				byte options = 0;
				if (PreviewData)
				{
					options |= PreviewDataMask;
				}
				if (StreamTerminated)
				{
					options |= StreamTerminatedMask;
				}
				if (ForceSynchronization)
				{
					options |= ForceSynchronizationMask;
				}
				return options;
			}
			set
			{
				PreviewData = (value & PreviewDataMask) != 0;
				StreamTerminated = (value & StreamTerminatedMask) != 0;
				ForceSynchronization = (value & ForceSynchronizationMask) != 0;
			}
		}

		public static DataPacketFramingLayer Parse(ReadOnlySpan<byte> bytes)
		{
			DataPacketFramingLayer framingLayer = new DataPacketFramingLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			framingLayer.Vector = (FramingLayerVector)BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			var sourceNameBytes = bytes.Slice(0, 64);
			bytes = bytes.Slice(64);
			framingLayer.SourceName = Encoding.UTF8.GetString(sourceNameBytes.ToArray());

			framingLayer.Priority = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			framingLayer.SynchronizationAddress = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			framingLayer.SequenceNumber = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			framingLayer.Options = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			framingLayer.Universe = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			return framingLayer;
		}
	}
}
