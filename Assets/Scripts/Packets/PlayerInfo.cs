namespace Packets
{
    public class PlayerInfo : IReadable, IWritable
    {
        public int Id { get; set; }
        public Position Position { get; set; }
        public Direction Direction { get; set; }

        public void Read(PacketReader reader)
        {
            Id = reader.ReadInt32();
            Position = new Position();
            Position.Read(reader);
            Direction = (Direction)reader.ReadByte();
        }

        public void Write(PacketWriter writer)
        {
            writer.Write(Id);
            Position.Write(writer);
            writer.Write((byte)Direction);
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Pos: {1}, Direction: {2}", Id, Position, Direction);
        }
    }
}