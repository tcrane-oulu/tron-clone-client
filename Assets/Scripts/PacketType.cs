public enum PacketType : byte
{
    Failure = 0,
    LobbyInfo = 1,
    LoadGame = 2,
    Update = 3,
    Tick = 4,
    StartGame = 5,
    Login = 6,
    LoadGameAck = 7,
    LobbyUpdate = 8,
}