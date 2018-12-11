namespace Packets
{
    public class EndGamePacket : IServerPacket
    {
        public PacketType Id
        {
            get { return PacketType.EndGame; }
        }

        public int WinnerId { get; set; }
        public string WinnerName { get; set; }

        public void Read(PacketReader reader)
        {
            WinnerId = reader.ReadInt32();
            WinnerName = reader.ReadString();
        }

        public override string ToString()
        {
            return string.Format("WinnerId: {0}, name: {1}", WinnerId, WinnerName);
        }
    }
}