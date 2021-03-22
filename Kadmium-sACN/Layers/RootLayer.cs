using System;
using System.Buffers.Binary;
using System.Collections;

namespace Kadmium_sACN.Layers
{
	public class RootLayer : SACNLayer
	{
		public const UInt32 VECTOR_ROOT_E131_DATA =		0x00000004;
		public const UInt32 VECTOR_ROOT_E131_EXTENDED =	0x00000008;

		public const UInt16 FLAGS =						0x7;
		public const UInt16 PREAMBLE_SIZE =				0x0010;
		public const UInt16 POSTAMBLE_SIZE =			0;
		public const int LENGTH =						38;

		public static readonly byte[] ACNIdentifier = new byte[]
		{
			0x41, 0x53, 0x43, 0x2d, 0x45, 0x31, 0x2e, 0x31, 0x37, 0x00, 0x00, 0x00
		};

		public UInt32 Vector { get; set; }
		public byte[] CID { get; set; } = new byte[16];
		public override int Length => LENGTH;

		public static RootLayer Parse(ReadOnlySpan<byte> bytes)
		{
			RootLayer rootLayer = new RootLayer();
			bytes = bytes.Slice(0, rootLayer.Length);

			var preambleSize = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (preambleSize != PREAMBLE_SIZE)
			{
				throw new ArgumentException($"The preamble size was not correct. Expected {PREAMBLE_SIZE}, received {preambleSize}");
			}

			var postambleSize = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (postambleSize != POSTAMBLE_SIZE)
			{
				throw new ArgumentException($"The postamble size was not correct. Expected {POSTAMBLE_SIZE}, received {postambleSize}");
			}

			var acnIdentifier = bytes.Slice(0, ACNIdentifier.Length);
			bytes = bytes.Slice(ACNIdentifier.Length);
			if (!acnIdentifier.SequenceEqual(ACNIdentifier))
			{
				throw new ArgumentException($"The sequence identifier was not correct");
			}

			rootLayer.FlagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if(rootLayer.Flags != FLAGS)
			{
				throw new ArgumentException($"The flags were not correct. Expected {FLAGS}, received {rootLayer.Flags}");
			}

			rootLayer.Vector = BinaryPrimitives.ReadUInt32BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt32));

			var cid = bytes.Slice(0, 16);
			cid.CopyTo(rootLayer.CID);

			return rootLayer;
		}
	}
}
