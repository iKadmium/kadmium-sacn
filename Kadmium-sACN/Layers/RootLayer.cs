using System;
using System.Buffers.Binary;
using System.Collections;

namespace Kadmium_sACN.Layers
{
	public class RootLayer : SACNLayer
	{
		public const UInt32 VECTOR_ROOT_E131_DATA =		0x00000004;
		public const UInt32 VECTOR_ROOT_E131_EXTENDED =	0x00000008;

		public static readonly byte[] ACNIdentifier = new byte[]
		{
			0x41, 0x53, 0x43, 0x2d, 0x45, 0x31, 0x2e, 0x31, 0x37, 0x00, 0x00, 0x00
		};

		public UInt32 Vector { get; set; }
		public byte[] CID { get; set; }
		public override int Length => 38;
		
		public static RootLayer Parse(ReadOnlySpan<byte> bytes)
		{
			RootLayer rootLayer = new RootLayer();

			var preambleSize = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0, 2));
			bytes = bytes.Slice(sizeof(UInt16));
			if (preambleSize != 0x0010)
			{
				throw new ArgumentException("The preamble size was not correct. Expected 16, received " + preambleSize);
			}

			var postambleSize = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0, 2));
			bytes = bytes.Slice(sizeof(UInt16));
			if (postambleSize != 0x0010)
			{
				throw new ArgumentException("The postamble size was not correct. Expected 0, received " + postambleSize);
			}

			var acnIdentifier = bytes.Slice(0, 12);
			if (!acnIdentifier.SequenceEqual(ACNIdentifier))
			{
				throw new ArgumentException($"The sequence identifier was not correct");
			}

			rootLayer.FlagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(0, rootLayer.FlagsAndLength);
			bytes = bytes.Slice(sizeof(UInt16));

			rootLayer.Vector = BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			var cid = bytes.Slice(0, 16);
			cid.CopyTo(rootLayer.CID);

			return rootLayer;
		}
	}
}
