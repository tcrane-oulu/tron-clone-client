using Packets;
public static class PacketResolver
{
    public static IServerPacket Create(PacketType type)
    {
        switch (type)
        {
            case PacketType.Failure: return new FailurePacket();
            case PacketType.LobbyInfo: return new LobbyInfoPacket();
            case PacketType.LoadGame: return new LoadGamePacket();
            case PacketType.Tick: return new TickPacket();
            case PacketType.Death: return new DeathPacket();
            case PacketType.EndGame: return new EndGamePacket();
        }
        return null;
    }
}