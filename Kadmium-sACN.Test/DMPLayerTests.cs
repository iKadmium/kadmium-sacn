using Kadmium_sACN.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class DMPLayerTests
	{
		[Fact]
		public void Given_TheDataIsCorrect_When_ParseIsCalled_Then_TheDataIsParsedAsExpected()
		{
			UInt16 propertyCount = 5;
			var expectedProperties = Enumerable.Range(0, propertyCount).Select(x => (byte)x);
			var bytes = GetDMPLayer(expectedProperties);

			var dmpLayer = DMPLayer.Parse(bytes.ToArray());
			Assert.Equal(DMPLayer.VECTOR_DMP_SET_PROPERTY, dmpLayer.Vector);
			Assert.Equal(propertyCount, dmpLayer.PropertyValues.Length);
			Assert.Equal(expectedProperties.ToArray(), dmpLayer.PropertyValues);
		}

		[Fact]
		public void Given_TheAddressTypeAndDataTypeAreIncorrect_When_ParseIsCalled_Then_NullIsReturned()
		{
			List<byte> bytes = new List<byte>();
			UInt16 propertyCount = 5;
			byte length = (byte)(DMPLayer.MIN_LENGTH + (propertyCount - 1));
			UInt16 expectedFlagsAndLength = (UInt16)(0x7 << 12 | length);
			bytes.AddRange(new byte[] { 0x7 << 4, (length) }); // flags and length
			byte expectedVector = DMPLayer.VECTOR_DMP_SET_PROPERTY;
			bytes.Add(expectedVector); // Vector
			byte expectedAddressAndDataType = 25;
			bytes.Add(expectedAddressAndDataType);

			bytes.AddRange(new byte[] { 0, 0 }); // first property address
			bytes.AddRange(new byte[] { 0, 1 }); // address increment
			bytes.AddRange(new byte[] { 0, (byte)propertyCount }); // property count

			var expectedProperties = Enumerable.Range(0, propertyCount).Select(x => (byte)x);
			bytes.Add(0); // start code
			bytes.AddRange(expectedProperties);

			var dmpLayer = DMPLayer.Parse(bytes.ToArray());
			Assert.Null(dmpLayer);
		}

		[Fact]
		public void Given_TheFirstPropertyAddressIsIncorrect_When_ParseIsCalled_Then_NullIsReturned()
		{
			List<byte> bytes = new List<byte>();
			UInt16 propertyCount = 5;
			byte length = (byte)(DMPLayer.MIN_LENGTH + (propertyCount - 1));
			UInt16 expectedFlagsAndLength = (UInt16)(0x7 << 12 | length);
			bytes.AddRange(new byte[] { 0x7 << 4, (length) }); // flags and length
			byte expectedVector = DMPLayer.VECTOR_DMP_SET_PROPERTY;
			bytes.Add(expectedVector); // Vector
			byte expectedAddressAndDataType = DMPLayer.AddressTypeAndDataType;
			bytes.Add(expectedAddressAndDataType);

			bytes.AddRange(new byte[] { 0, 42 }); // first property address
			bytes.AddRange(new byte[] { 0, 1 }); // address increment
			bytes.AddRange(new byte[] { 0, (byte)propertyCount }); // property count

			var expectedProperties = Enumerable.Range(0, propertyCount).Select(x => (byte)x);
			bytes.Add(0); // start code
			bytes.AddRange(expectedProperties);

			var dmpLayer = DMPLayer.Parse(bytes.ToArray());
			Assert.Null(dmpLayer);
		}

		[Fact]
		public void Given_TheAddressIncrementIsIncorrect_When_ParseIsCalled_Then_NullIsReturned()
		{
			List<byte> bytes = new List<byte>();
			UInt16 propertyCount = 5;
			byte length = (byte)(DMPLayer.MIN_LENGTH + (propertyCount - 1));
			UInt16 expectedFlagsAndLength = (UInt16)(0x7 << 12 | length);
			bytes.AddRange(new byte[] { 0x7 << 4, (length) }); // flags and length
			byte expectedVector = DMPLayer.VECTOR_DMP_SET_PROPERTY;
			bytes.Add(expectedVector); // Vector
			byte expectedAddressAndDataType = DMPLayer.AddressTypeAndDataType;
			bytes.Add(expectedAddressAndDataType);

			bytes.AddRange(new byte[] { 0, 0 }); // first property address
			bytes.AddRange(new byte[] { 0, 42 }); // address increment
			bytes.AddRange(new byte[] { 0, (byte)propertyCount }); // property count

			var expectedProperties = Enumerable.Range(0, propertyCount).Select(x => (byte)x);
			bytes.Add(0); // start code
			bytes.AddRange(expectedProperties);

			var dmpLayer = DMPLayer.Parse(bytes.ToArray());
			Assert.Null(dmpLayer);
		}

		public static List<byte> GetDMPLayer(IEnumerable<byte> properties)
		{
			List<byte> bytes = new List<byte>();
			UInt16 propertyCount = (UInt16)properties.Count();
			byte length = (byte)(DMPLayer.MIN_LENGTH + (propertyCount - 1));
			bytes.AddRange(new byte[] { 0x7 << 4, (length) }); // flags and length
			byte expectedVector = DMPLayer.VECTOR_DMP_SET_PROPERTY;
			bytes.Add(expectedVector); // Vector
			byte expectedAddressAndDataType = DMPLayer.AddressTypeAndDataType;
			bytes.Add(expectedAddressAndDataType);

			bytes.AddRange(new byte[] { 0, 0 }); // first property address
			bytes.AddRange(new byte[] { 0, 1 }); // address increment
			bytes.AddRange(new byte[] { 0, (byte)(propertyCount + 1) }); // property count

			bytes.Add(0); // start code
			bytes.AddRange(properties);

			return bytes;
		}
	}
}
