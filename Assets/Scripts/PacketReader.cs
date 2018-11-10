using System.IO;
using System.Text;
using System.Net;
using System;

public class PacketReader : BinaryReader
{
    public PacketReader(MemoryStream input) : base(input, Encoding.UTF8)
    {

    }

    // Reads an int32 (4 bytes) from the packet buffer.
    public override int ReadInt32()
    {
        return IPAddress.NetworkToHostOrder(base.ReadInt32());
    }

    // Reads an int16 (2 bytes) from the packet buffer.
    public override short ReadInt16()
    {
        return IPAddress.NetworkToHostOrder(base.ReadInt16());
    }

    public override float ReadSingle()
    {
        byte[] single = base.ReadBytes(4);
        Array.Reverse(single);
        return BitConverter.ToSingle(single, 0);
    }

    // Reads a string from the packet buffer.
    public override string ReadString()
    {
        var strLen = ReadInt16();
        return Encoding.UTF8.GetString(ReadBytes(strLen));
    }
}