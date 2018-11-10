public interface IPacket
{
    PacketType Id { get; }
}

public interface IClientPacket : IPacket, IWritable { }
public interface IServerPacket : IPacket, IReadable { }

public interface IReadable
{
    void Read(PacketReader reader);
}

public interface IWritable
{
    void Write(PacketWriter writer);
}