using Kadmium_sACN.MulticastAddressProvider;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test.MulticastAddressProvider
{
	public class SacnMulticastAddressProviderIPV6Tests
	{
		[Theory]
		[InlineData(1, "FF18::83:00:00:01")]
		[InlineData(256, "FF18::83:00:01:00")]
		[InlineData(63999, "FF18::83:00:F9:FF")]
		public void When_GetIPV6MulticastAddressIsCalled_Then_TheResultIsAsExpected(UInt16 universe, string expectedAddressString)
		{
			var addressProvider = new SacnMulticastAddressProviderIPV6();
			var actual = addressProvider.GetMulticastAddress(universe);
			var expected = IPAddress.Parse(expectedAddressString);

			Assert.Equal(expected, actual);
		}
	}
}
