namespace Packets
{
    public class LoadGameAckPacket : IClientPacket
    {

        public int ClientId { get; set; }
        public PacketType Id
        {
            get { return PacketType.LoadGameAck; }
        }

        public void Write(PacketWriter writer)
        {
            writer.Write(ClientId);
        }

        public override string ToString()
        {
            return string.Format("ClientId: {0}", ClientId);
        }
    }
}