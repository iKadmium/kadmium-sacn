using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Kadmium_sACN.Layers
{
	public enum DMPLayerVector
	{
		VECTOR_DMP_SET_PROPERTY = 0x02
	}

	public class DMPLayer : SACNLayer
	{
		public const UInt16 MinLength = 11;

		public const byte AddressTypeAndDataType = 0xa1;
		public const UInt16 FirstPropertyAddress = 0x00;
		public const UInt16 AddressIncrement = 0x01;

		public DMPLayerVector Vector { get; set; }
		public byte StartCode { get; set; }
		public IEnumerable<byte> PropertyValues { get; set; }
		public UInt16 Length => (UInt16)(MinLength + PropertyValues.Count());

		public DMPLayer()
		{
			Vector = DMPLayerVector.VECTOR_DMP_SET_PROPERTY;
		}

		public void Write(Span<byte> bytes)
		{
			BinaryPrimitives.WriteUInt16BigEndian(bytes, GetFlagsAndLength(Length));
			bytes = bytes.Slice(sizeof(UInt16));
			bytes[0] = (byte)Vector;
			bytes = bytes.Slice(sizeof(byte));
			bytes[0] = AddressTypeAndDataType;
			bytes = bytes.Slice(sizeof(byte));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, FirstPropertyAddress);
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, AddressIncrement);
			bytes = bytes.Slice(sizeof(UInt16));
			BinaryPrimitives.WriteUInt16BigEndian(bytes, (UInt16)(PropertyValues.Count() + 1));
			bytes = bytes.Slice(sizeof(UInt16));
			bytes[0] = StartCode;
			bytes = bytes.Slice(sizeof(byte));
			PropertyValues.ToArray().CopyTo(bytes);
		}

		public static DMPLayer Parse(ReadOnlySpan<byte> bytes)
		{
			DMPLayer dmpLayer = new DMPLayer();

			var flagsAndLength = BinaryPrimitives.ReadUInt16BigEndian(bytes);
			bytes = bytes.Slice(sizeof(UInt16));

			dmpLayer.Vector = (DMPLayerVector)bytes[0];
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

			var propertyValues = new byte[propertyValueCount - 1];

			var propertyValuesSlice = bytes.Slice(0, propertyValueCount - 1);
			propertyValuesSlice.CopyTo(propertyValues);
			dmpLayer.PropertyValues = propertyValues;

			return dmpLayer;
		}
	}
}
