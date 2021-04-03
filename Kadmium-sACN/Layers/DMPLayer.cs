using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public class DMPLayer : SACNLayer
	{
		public const byte VECTOR_DMP_SET_PROPERTY = 0x02;
		public const int MIN_LENGTH = 12;

		public const byte AddressTypeAndDataType = 0xa1;
		public const UInt16 FirstPropertyAddress = 0x00;
		public const UInt16 AddressIncrement = 0x01;

		public byte Vector { get; set; }
		public byte StartCode { get; set; }
		public byte[] PropertyValues { get; set; }

		public override int Length => MIN_LENGTH + PropertyValues.Length;

		public static DMPLayer Parse(ReadOnlySpan<byte> bytes)
		{
			DMPLayer dmpLayer = new DMPLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			dmpLayer.Vector = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			var addressTypeAndDataType = bytes[0];
			bytes = bytes.Slice(sizeof(byte));
			if (addressTypeAndDataType != AddressTypeAndDataType)
			{
				return null;
			}

			var firstPropertyAddress = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (firstPropertyAddress != FirstPropertyAddress)
			{
				return null;
			}

			var addressIncrement = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (addressIncrement != AddressIncrement)
			{
				return null;
			}

			var propertyValueCount = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			dmpLayer.StartCode = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			dmpLayer.PropertyValues = new byte[propertyValueCount - 1];
			var propertyValues = bytes.Slice(0, propertyValueCount - 1);
			propertyValues.CopyTo(dmpLayer.PropertyValues);

			return dmpLayer;
		}
	}
}
