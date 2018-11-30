public enum PacketType : byte
{
    Failure = 0,
    LobbyInfo = 1,
    LoadGame = 2,
    Update = 3,
    Tick = 4,
    StartGame = 5,
    Death = 6,
    EndGame = 7,
    Login = 8,
    LoadGameAck = 9,
    LobbyUpdate = 10,
    PlayerUpdate = 11,
}