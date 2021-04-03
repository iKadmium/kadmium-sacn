using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kadmium_sACN.Test
{
	public class SACNPacketTests
	{
		[Fact]
		public void Given_ThePacketIsAValidDataPacket_When_ParseIsCalled_Then_ThePacketIsParsedCorrectly()
		{
			DataPacket packet = new DataPacket();
		}
	}
}
