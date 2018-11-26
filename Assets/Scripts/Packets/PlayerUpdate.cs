namespace Packets
{
    public class PlayerUpdatePacket : IClientPacket
    {
        public PacketType Id
        {
            get { return PacketType.PlayerUpdate; }
        }

        public Direction newDirection { get; set; }

        public PlayerUpdatePacket(Direction dir)
        {
            newDirection = dir;
        }

        public void Write(PacketWriter writer)
        {
            writer.Write((byte)newDirection);
        }

        public override string ToString()
        {
            return string.Format("Direction: {0}", newDirection);
        }
    }
}