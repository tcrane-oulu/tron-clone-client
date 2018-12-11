using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Packets;

public class Player : MonoBehaviour
{

    public NetworkClient client;
    private Queue<IServerPacket> serverPackets;
    private Object mutex = new Object();
    private PlayerManager playerManager;
    private LoginUI loginUI;

    private void OnDestroy()
    {
        client.Dispose();
    }

    public void Send(IClientPacket packet)
    {
        client.Send(packet);
    }

    public void Login(string name, int version, string ip = "35.228.234.250", int port = 2050)
    {
        Debug.Log(string.Format("Conneting to {0}:{1}", ip, port));
        client.Connect(ip, 2050);
        client.Connected += (connected) =>
        {
            if (!connected)
            {
                Debug.Log("Couldn't connect.");
                return;
            }
            Debug.Log("Connected!");
            var login = new Packets.LoginPacket();
            login.Name = name;
            login.Version = version;
            client.Send(login);
        };
    }

    // Use this for initialization
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        loginUI = FindObjectOfType<LoginUI>();
        serverPackets = new Queue<IServerPacket>();
        client = new NetworkClient();

        client.OnPacket += (packet) =>
        {
            lock (mutex)
            {
                serverPackets.Enqueue(packet);
            }
        };
    }
    private void Update()
    {
        IServerPacket packet = null;
        lock (mutex)
        {
            if (serverPackets.Count > 0)
            {
                packet = serverPackets.Dequeue();
            }
        }
        if (packet != null)
        {
            switch (packet.Id)
            {
                case PacketType.LobbyInfo:
                    loginUI.OnLobbyInfo(packet as LobbyInfoPacket);
                    break;
                case PacketType.StartGame:
                    loginUI.OnStartGame(packet as StartGamePacket);
                    break;
                case PacketType.LoadGame:
                    var myId = (packet as LoadGamePacket).ClientId;
                    var ack = new LoadGameAckPacket();
                    ack.ClientId = myId;
                    client.Send(ack);
                    loginUI.OnLoadGame();
                    playerManager.OnLoadGame(packet as LoadGamePacket);
                    break;
                case PacketType.Tick:
                    // str: Players: ID: 0, Pos:(float, float), Direction: Right, 
                    // Id: 1, Pos (float, float), direction: Right, ...
                    playerManager.OnTick(packet as TickPacket);
                    break;
                case PacketType.Death:
                    playerManager.OnDeath(packet as DeathPacket);
                    break;
                case PacketType.EndGame:
                    loginUI.OnEndGame(packet as EndGamePacket);
                    playerManager.OnEndGame(packet as EndGamePacket);
                    break;
                default:
                    Debug.Log(string.Format("Type: {0}, str: {1}", packet.Id, packet.ToString()));
                    break;
            }
        }
    }
}
