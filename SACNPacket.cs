using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace kadmium_sacn
{
    public class SACNPacket
    {
        public static Int16 FLAGS = (0x7 << 12);
        public static UInt16 FIRST_FOUR_BITS_MASK = 61440;
        public static UInt16 LAST_TWELVE_BITS_MASK = 4095;

        public RootLayer RootLayer { get; set; }

        public string SourceName { get { return RootLayer.FramingLayer.SourceName; } set { RootLayer.FramingLayer.SourceName = value; } }
        public Guid UUID { get { return RootLayer.UUID; } set { RootLayer.UUID = value; } }
        public byte SequenceID { get { return RootLayer.FramingLayer.SequenceID; } set { RootLayer.FramingLayer.SequenceID = value; } }
        public byte[] Data { get { return RootLayer.FramingLayer.DMPLayer.Data; } set { RootLayer.FramingLayer.DMPLayer.Data = value; } }
        public Int16 UniverseID { get { return RootLayer.FramingLayer.UniverseID; } set { RootLayer.FramingLayer.UniverseID = value; } }
        
        public SACNPacket(Int16 universeID, String sourceName, Guid uuid, byte sequenceID, byte[] data)
        {
            RootLayer = new RootLayer(uuid, sourceName, universeID, sequenceID, data);
        }

        public SACNPacket(RootLayer rootLayer)
        {
            RootLayer = rootLayer;
        }

        public static SACNPacket Parse(byte[] packet)
        {
            MemoryStream stream = new MemoryStream(packet);
            BigEndianBinaryReader buffer = new BigEndianBinaryReader(stream);

            RootLayer rootLayer = RootLayer.Parse(buffer);
            
            return new SACNPacket(rootLayer);
        }

        public byte[] ToArray()
        {
            return RootLayer.ToArray();
        }
    }

    public class RootLayer
    {
        static Int16 PREAMBLE_LENGTH = 0x0010;
        static Int16 POSTAMBLE_LENGTH = 0x0000;
        static byte[] PACKET_IDENTIFIER = new byte[] {0x41, 0x53, 0x43, 0x2d, 0x45,
                                             0x31, 0x2e, 0x31, 0x37, 0x00,
                                             0x00, 0x00};
        static Int32 ROOT_VECTOR = 0x00000004;

        public FramingLayer FramingLayer { get; set; }
        public Int16 Length { get { return (Int16)(38 + FramingLayer.Length); }  }
        public Guid UUID { get; set; }

        public RootLayer(Guid uuid, string sourceName, Int16 universeID, byte sequenceID, byte[] data)
        {
            FramingLayer = new FramingLayer(sourceName, universeID, sequenceID, data);
        }

        public RootLayer()
        {

        }

        public byte[] ToArray()
        {
            MemoryStream stream = new MemoryStream(Length);
            BinaryWriter buffer = new BigEndianBinaryWriter(stream);

            buffer.Write(PREAMBLE_LENGTH);
            buffer.Write(POSTAMBLE_LENGTH);
            buffer.Write(PACKET_IDENTIFIER);
            Int16 flagsAndRootLength = (Int16)(SACNPacket.FLAGS | Length);
            buffer.Write(flagsAndRootLength);
            buffer.Write(ROOT_VECTOR);
            buffer.Write(UUID.ToByteArray());

            buffer.Write(FramingLayer.ToArray());
            return stream.ToArray();
        }

        internal static RootLayer Parse(BigEndianBinaryReader buffer)
        {
            RootLayer rootLayer = new RootLayer();

            Int16 preambleLength = buffer.ReadInt16();
            Debug.Assert(preambleLength == PREAMBLE_LENGTH);
            Int16 postambleLength = buffer.ReadInt16();
            Debug.Assert(postambleLength == POSTAMBLE_LENGTH);
            byte[] packetIdentifier = buffer.ReadBytes(12);
            Debug.Assert(packetIdentifier.SequenceEqual(PACKET_IDENTIFIER));
            UInt16 flagsAndRootLength = (UInt16)buffer.ReadInt16();
            UInt16 flags = (UInt16)(flagsAndRootLength & SACNPacket.FIRST_FOUR_BITS_MASK);
            Debug.Assert(flags == SACNPacket.FLAGS);
            UInt16 length = (UInt16)(flagsAndRootLength & SACNPacket.LAST_TWELVE_BITS_MASK);
            Int32 vector = buffer.ReadInt32();
            Debug.Assert(vector == ROOT_VECTOR);
            Guid cid = new Guid(buffer.ReadBytes(16));

            rootLayer.UUID = cid;
            rootLayer.FramingLayer = FramingLayer.Parse(buffer);

            return rootLayer;
        }
    }

    public class FramingLayer
    {
        static Int32 FRAMING_VECTOR = 0x00000002;
        static byte PRIORITY = 100;
        static Int16 RESERVED = 0;
        static byte OPTIONS = 0;

        public DMPLayer DMPLayer { get; set; }
        public Int16 Length { get { return (Int16)(77 + DMPLayer.Length); } }
        public string SourceName { get; set; }
        public Int16 UniverseID { get; set; }
        public byte SequenceID { get; set; }

        public FramingLayer(string sourceName, Int16 universeID, byte sequenceID, byte[] data)
        {
            SourceName = sourceName;
            UniverseID = universeID;
            SequenceID = sequenceID;
            DMPLayer = new DMPLayer(data);
        }

        public FramingLayer()
        {

        }

        public byte[] ToArray()
        {
            MemoryStream stream = new MemoryStream(Length);
            BinaryWriter buffer = new BigEndianBinaryWriter(stream);

            Int16 flagsAndFramingLength = (Int16)(SACNPacket.FLAGS | Length);
            buffer.Write(flagsAndFramingLength);
            buffer.Write(FRAMING_VECTOR);
            buffer.Write(Encoding.UTF8.GetBytes(SourceName));
            for (int i = 0; i < 64 - SourceName.Length; i++)
            {
                buffer.Write((byte)0);
            }
            buffer.Write(PRIORITY);
            buffer.Write(RESERVED);
            buffer.Write(SequenceID);
            buffer.Write(OPTIONS);
            buffer.Write(UniverseID);

            buffer.Write(DMPLayer.ToArray());

            return stream.ToArray();
        }

        internal static FramingLayer Parse(BigEndianBinaryReader buffer)
        {
            UInt16 flagsAndFramingLength = (UInt16)buffer.ReadInt16();
            UInt16 flags = (UInt16)(flagsAndFramingLength & SACNPacket.FIRST_FOUR_BITS_MASK);
            Debug.Assert(flags == SACNPacket.FLAGS);
            UInt16 length = (UInt16)(flagsAndFramingLength & SACNPacket.LAST_TWELVE_BITS_MASK);

            Int32 vector2 = buffer.ReadInt32();
            Debug.Assert(vector2 == FRAMING_VECTOR);
            byte[] sourceNameBytes = buffer.ReadBytes(64);
            string sourceName = new string(Encoding.UTF8.GetChars(sourceNameBytes)).TrimEnd('\0');
            byte priority = buffer.ReadByte();
            Int16 reserved = buffer.ReadInt16();
            byte sequenceID = buffer.ReadByte();
            byte options = buffer.ReadByte();
            Int16 universeID = buffer.ReadInt16();

            FramingLayer framingLayer = new FramingLayer();
            framingLayer.SequenceID = sequenceID;
            framingLayer.SourceName = sourceName;
            framingLayer.DMPLayer = DMPLayer.Parse(buffer);

            return framingLayer;
        }
    }

    public class DMPLayer
    {
        static byte DMP_VECTOR = 2;
        static byte ADDRESS_TYPE_AND_DATA_TYPE = 0xa1;
        static Int16 FIRST_PROPERTY_ADDRESS = 0;
        static Int16 ADDRESS_INCREMENT = 1;
        static byte ZERO_ADDRESS = 0x00;

        public Int16 Length { get { return (Int16)(10 + Data.Length); } }

        public byte[] Data { get; set; }

        public DMPLayer(byte[] data)
        {
            Data = data;
        }
        
        public byte[] ToArray()
        {
            MemoryStream stream = new MemoryStream(Length);
            BinaryWriter buffer = new BigEndianBinaryWriter(stream);

            Int16 flagsAndDMPLength = (Int16)(SACNPacket.FLAGS | Length);

            buffer.Write(flagsAndDMPLength);
            buffer.Write(DMP_VECTOR);
            buffer.Write(ADDRESS_TYPE_AND_DATA_TYPE);
            buffer.Write(FIRST_PROPERTY_ADDRESS);
            buffer.Write(ADDRESS_INCREMENT);
            buffer.Write((Int16)(Data.Length + 1));
            buffer.Write(ZERO_ADDRESS);
            buffer.Write(Data);

            return stream.ToArray();
        }

        internal static DMPLayer Parse(BigEndianBinaryReader buffer)
        {
            Int16 flagsAndDMPLength = buffer.ReadInt16();
            byte vector3 = buffer.ReadByte();
            Debug.Assert(vector3 == DMP_VECTOR);
            byte addressTypeAndDataType = buffer.ReadByte();
            Debug.Assert(addressTypeAndDataType == ADDRESS_TYPE_AND_DATA_TYPE);
            Int16 firstPropertyAddress = buffer.ReadInt16();
            Debug.Assert(firstPropertyAddress == FIRST_PROPERTY_ADDRESS);
            Int16 addressIncrement = buffer.ReadInt16();
            Debug.Assert(addressIncrement == ADDRESS_INCREMENT);
            Int16 propertyValueCount = buffer.ReadInt16();

            byte startCode = buffer.ReadByte();
            byte[] properties = buffer.ReadBytes(propertyValueCount - 1);

            DMPLayer dmpLayer = new DMPLayer(properties);
            return dmpLayer;
        }
    }
}
