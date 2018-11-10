namespace Packets
{
    public class LoadGamePacket : IServerPacket
    {
        public PacketType Id
        {
            get { return PacketType.LoadGame; }
        }

        public byte ClientId { get; set; }
        public short MapSize { get; set; }

        public void Read(PacketReader reader)
        {
            ClientId = reader.ReadByte();
            MapSize = reader.ReadInt16();
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, MapSize: {1}", ClientId, MapSize);
        }
    }
}