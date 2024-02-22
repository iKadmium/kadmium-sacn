using System;
using System.Collections.Generic;
using System.Text;

namespace Kadmium_sACN
{
	public static class Constants
	{
		public const int RemotePort = 5568;
		public const int LocalPort = 5569;
		public const int MaxPacketLength = 1444;
		public const int Universe_MinValue = 1;
		public const int Universe_MaxValue = 63999;
		public const int Priority_MaxValue = 200;
		public const byte Priority_Default = 100;
	}
}
