namespace Packets
{
    public class StartGamePacket : IServerPacket
    {
        public PacketType Id
        {
            get { return PacketType.StartGame; }
        }

        public int StartTime { get; set; }

        public void Read(PacketReader reader)
        {
            StartTime = reader.ReadInt32();
        }

        public override string ToString()
        {
            return string.Format("StartTime: {0}", StartTime);
        }
    }
}