namespace Packets
{
    public class Position : IReadable, IWritable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public void Read(PacketReader reader)
        {
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
        }

        public void Write(PacketWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", X, Y);
        }
    }
}