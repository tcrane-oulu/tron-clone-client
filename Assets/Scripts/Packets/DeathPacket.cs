namespace Packets
{
    public class DeathPacket : IServerPacket
    {
        public PacketType Id
        {
            get { return PacketType.Death; }
        }

        public int PlayerId { get; set; }

        public void Read(PacketReader reader)
        {
            PlayerId = reader.ReadInt32();
        }

        public override string ToString()
        {
            return string.Format("PlayerId: {0}", PlayerId);
        }
    }
}