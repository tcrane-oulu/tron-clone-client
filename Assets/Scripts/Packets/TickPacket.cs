namespace Packets
{
    public class TickPacket : IServerPacket
    {
        public PlayerInfo[] Players;

        public PacketType Id
        {
            get { return PacketType.Tick; }
        }
        

        public void Read(PacketReader reader)
        {
            short size = reader.ReadInt16();
            Players = new PlayerInfo[size];
            for (int i = 0; i < size; i++)
            {
                Players[i] = new PlayerInfo();
                Players[i].Read(reader);
            }
        }

        public override string ToString()
        {
            string str = "";
            foreach (var player in Players)
            {
                str += player.ToString() + ", ";
            }
            return string.Format("Players: {0}", str);
        }
    }
}