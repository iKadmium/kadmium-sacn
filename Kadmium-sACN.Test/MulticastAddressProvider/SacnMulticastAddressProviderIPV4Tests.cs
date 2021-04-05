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
	public class SacnMulticastAddressProviderIPV4Tests
	{
		[Theory]
		[InlineData(1, "239.255.0.1")]
		[InlineData(256, "239.255.1.0")]
		[InlineData(63999, "239.255.249.255")]
		public void When_GetIPV4MulticastAddressIsCalled_Then_TheResultIsAsExpected(UInt16 universe, string expectedAddressString)
		{
			var addressProvider = new SacnMulticastAddressProviderIPV4();
			var actual = addressProvider.GetMulticastAddress(universe);
			var expected = IPAddress.Parse(expectedAddressString);

			Assert.Equal(expected, actual);
		}
	}
}
