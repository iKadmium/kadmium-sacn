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

		public const int Length = 77;

		public const int Universe_MinValue = 1;
		public const int Universe_MaxValue = 63999;
		public const int Priority_MaxValue = 200;

		public string SourceName { get; set; }
		private byte priority = 100;
		public byte Priority
		{
			get { return priority; }
			set
			{
				if (value > Priority_MaxValue)
				{
					throw new ArgumentOutOfRangeException($"Priority may not exceed {Priority_MaxValue}");
				}
				priority = value;
			}
		}
		public UInt16 SynchronizationAddress { get; set; }
		public byte SequenceNumber { get; set; }
		public bool PreviewData { get; set; }
		public bool StreamTerminated { get; set; }
		public bool ForceSynchronization { get; set; }
		private UInt16 universe;
		public UInt16 Universe
		{
			get { return universe; }
			set
			{
				if (value < Universe_MinValue || value > Universe_MaxValue)
				{
					throw new ArgumentOutOfRangeException($"Universe must be between {Universe_MinValue} and {Universe_MaxValue}");
				}
				universe = value;
			}
		}
		
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

		

		public DataPacketFramingLayer()
		{
			Vector = FramingLayerVector.VECTOR_E131_DATA_PACKET;
		}

		public void Write(Span<byte> bytes, ushort remainingLength)
		{
			UInt16 pduLength = (UInt16)(remainingLength + Length);
			BinaryPrimitives.WriteUInt16BigEndian(bytes, GetFlagsAndLength(pduLength));
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt32BigEndian(bytes, (UInt32)Vector);
			bytes = bytes.Slice(sizeof(UInt32));
			var sourceNameBytes = Encoding.UTF8.GetBytes(SourceName);
			sourceNameBytes.CopyTo(bytes);
			bytes = bytes.Slice(sourceNameBytes.Length);
			var paddingLength = 64 - sourceNameBytes.Length;
			bytes.Slice(0, paddingLength).Fill(0);
			bytes = bytes.Slice(paddingLength);
			bytes[0] = Priority;
			bytes = bytes.Slice(sizeof(byte));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, SynchronizationAddress);
			bytes = bytes.Slice(sizeof(UInt16));
			bytes[0] = SequenceNumber;
			bytes = bytes.Slice(sizeof(byte));
			bytes[0] = Options;
			bytes = bytes.Slice(sizeof(byte));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, Universe);
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
