using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public class DMPLayer : SACNLayer
	{
		public static byte VECTOR_DMP_SET_PROPERTY { get; } = 0x02;

		public static byte AddressTypeAndDataType { get; } = 0xa1;
		public static UInt16 FirstPropertyAddress { get; } = 0x00;
		public static UInt16 AddressIncrement { get; } = 0x01;

		public byte Vector { get; set; }
		public byte StartCode { get; set; }
		public byte[] PropertyValues { get; set; }

		public override int Length => PDULength;

		public static DMPLayer Parse(ReadOnlySpan<byte> bytes)
		{
			DMPLayer dmpLayer = new DMPLayer();

			dmpLayer.FlagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			dmpLayer.Vector = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			var addressTypeAndDataType = bytes[0];
			bytes = bytes.Slice(sizeof(byte));
			if (addressTypeAndDataType != AddressTypeAndDataType)
			{
				throw new ArgumentException($"Address type and data type were not correct. Expected {AddressTypeAndDataType}, received {addressTypeAndDataType}");
			}

			var firstPropertyAddress = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (firstPropertyAddress != 0)
			{
				throw new ArgumentException($"First property address was not correct. Expected {FirstPropertyAddress}, received {firstPropertyAddress}");
			}

			var addressIncrement = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));
			if (addressIncrement != 0)
			{
				throw new ArgumentException($"Address increment was not correct. Expected {AddressIncrement}, received {addressIncrement}");
			}

			var propertyValueCount = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			dmpLayer.StartCode = bytes[0];
			bytes = bytes.Slice(sizeof(byte));

			dmpLayer.PropertyValues = new byte[propertyValueCount - 1];
			var propertyValues = bytes.Slice(propertyValueCount - 1);
			propertyValues.CopyTo(dmpLayer.PropertyValues);

			return dmpLayer;
		}
	}
}
