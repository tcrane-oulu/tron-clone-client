namespace Packets
{
    public class LobbyInfo : IReadable, IWritable
    {
        public string Name { get; set; }
        public bool Ready { get; set; }

        public void Read(PacketReader reader)
        {
            Name = reader.ReadString();
            Ready = reader.ReadBoolean();
        }

        public void Write(PacketWriter writer)
        {
            writer.Write(Name);
            writer.Write(Ready);
        }

        public override string ToString()
        {
            return string.Format("{0}, ready = {1}", Name, Ready);
        }
    }
}