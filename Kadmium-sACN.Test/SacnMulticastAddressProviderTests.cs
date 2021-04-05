using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SacnMulticastAddressProviderTests
	{
		[Theory]
		[InlineData(1, "239.255.0.1")]
		[InlineData(256, "239.255.1.0")]
		[InlineData(63999, "239.255.249.255")]
		public void When_GetIPV4MulticastAddressIsCalled_Then_TheResultIsAsExpected(UInt16 universe, string expectedAddressString)
		{
			SacnMulticastAddressProvider addressProvider = new SacnMulticastAddressProvider();
			var actual = addressProvider.GetIPV4MulticastAddress(universe);
			var expected = IPAddress.Parse(expectedAddressString);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(1, "FF18::83:00:00:01")]
		[InlineData(256, "FF18::83:00:01:00")]
		[InlineData(63999, "FF18::83:00:F9:FF")]
		public void When_GetIPV6MulticastAddressIsCalled_Then_TheResultIsAsExpected(UInt16 universe, string expectedAddressString)
		{
			SacnMulticastAddressProvider addressProvider = new SacnMulticastAddressProvider();
			var actual = addressProvider.GetIPV6MulticastAddress(universe);
			var expected = IPAddress.Parse(expectedAddressString);

			Assert.Equal(expected, actual);
		}
	}
}
