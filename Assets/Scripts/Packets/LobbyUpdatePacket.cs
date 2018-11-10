namespace Packets
{
    public class LobbyUpdatePacket : IClientPacket
    {
        public PacketType Id
        {
            get { return PacketType.LobbyUpdate; }
        }

        public bool Ready { get; set; }

        public void Write(PacketWriter writer)
        {
            writer.Write(Ready);
        }

        public override string ToString()
        {
            return string.Format("Ready: {0}", Ready);
        }
    }
}