using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Specialized;

namespace Kadmium_sACN.Layers
{
	public enum RootLayerVector
	{
		VECTOR_ROOT_E131_DATA = 0x00000004,
		VECTOR_ROOT_E131_EXTENDED = 0x00000008
	}

	public class RootLayer : SACNLayer
	{
		public const UInt16 PREAMBLE_SIZE =				0x0010;
		public const UInt16 POSTAMBLE_SIZE =			0;
		public const int Length =						38;

		public static readonly byte[] ACNIdentifier = new byte[]
		{
			0x41, 0x53, 0x43, 0x2d, 0x45, 0x31, 0x2e, 0x31, 0x37, 0x00, 0x00, 0x00
		};

		public RootLayerVector Vector { get; set; }
		public byte[] CID { get; set; } = new byte[16];
		
		public void Write(Span<byte> bytes, UInt16 remainingLength)
		{
			BinaryPrimitives.WriteUInt16BigEndian(bytes, PREAMBLE_SIZE);
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, POSTAMBLE_SIZE);
			bytes = bytes.Slice(sizeof(UInt16));
			ACNIdentifier.CopyTo(bytes);
			bytes = bytes.Slice(ACNIdentifier.Length);
			UInt16 pduLength = (UInt16)(remainingLength + Length - 16);
			BinaryPrimitives.WriteUInt16BigEndian(bytes, GetFlagsAndLength(pduLength));
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt32BigEndian(bytes, (UInt32)Vector);
			bytes = bytes.Slice(sizeof(UInt32));
			CID.CopyTo(bytes);
		}

		public static RootLayer Parse(ReadOnlySpan<byte> bytes)
		{
			RootLayer rootLayer = new RootLayer();
			bytes = bytes.Slice(0, RootLayer.Length);

			var preambleSize = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (preambleSize != PREAMBLE_SIZE)
			{
				return null;
			}

			var postambleSize = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (postambleSize != POSTAMBLE_SIZE)
			{
				return null;
			}

			var acnIdentifier = bytes.Slice(0, ACNIdentifier.Length);
			bytes = bytes.Slice(ACNIdentifier.Length);
			if (!acnIdentifier.SequenceEqual(ACNIdentifier))
			{
				return null;
			}

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			
			rootLayer.Vector = (RootLayerVector)BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			var cid = bytes.Slice(0, 16);
			cid.CopyTo(rootLayer.CID);

			return rootLayer;
		}
	}
}
