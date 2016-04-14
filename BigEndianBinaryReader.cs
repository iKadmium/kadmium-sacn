using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace kadmium_sacn
{
    class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input) : base(input)
        {

        }

        public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding)
        {

        }

        public override short ReadInt16()
        {
            byte[] bytes = base.ReadBytes(2);
            Int16 converted = BitConverter.ToInt16(bytes, 0);
            return System.Net.IPAddress.NetworkToHostOrder(converted);
        }

        public override int ReadInt32()
        {
            byte[] bytes = base.ReadBytes(4);
            Int32 converted = BitConverter.ToInt32(bytes, 0);
            return System.Net.IPAddress.NetworkToHostOrder(converted); ;
        }
    }
}
