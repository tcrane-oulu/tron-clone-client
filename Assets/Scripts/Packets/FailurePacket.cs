namespace Packets
{
    public class FailurePacket : IServerPacket
    {
        public PacketType Id
        {
            get { return PacketType.Failure; }
        }

        public byte Code { get; set; }
        public string Description { get; set; }

        public void Read(PacketReader reader)
        {
            Code = reader.ReadByte();
            Description = reader.ReadString();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Code, Description);
        }
    }
}