using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Packets;

public class TestScript : MonoBehaviour
{

    private NetworkClient client;
    private Queue<IServerPacket> serverPackets;
    private Object mutex = new Object();
    private PlayerManager playerManager;

    private void OnDestroy()
    {
        client.Dispose();
    }

    public void Send(IClientPacket packet)
    {
        client.Send(packet);
    }

    // Use this for initialization
    void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        serverPackets = new Queue<IServerPacket>();
        client = new NetworkClient();
        client.Connect("localhost", 2050);

        client.OnPacket += (packet) =>
        {
            lock (mutex)
            {
                serverPackets.Enqueue(packet);
            }
        };
        client.Connected += (connected) =>
        {
            if (!connected)
            {
                return;
            }
            var login = new Packets.LoginPacket();
            login.Name = "Test Client";
            login.Version = 12345;
            client.Send(login);
        };
    }
    private void Update()
    {
        IServerPacket packet = null;
        lock (mutex)
        {
            //Debug.Log(string.Format("Queue length: {0}, deltatime {1}", serverPackets.Count, Time.deltaTime));
            if (serverPackets.Count > 0)
            {
                packet = serverPackets.Dequeue();
            }
        }
        if (packet != null)
        {
            // Debug.Log(string.Format("Received Server Packet {0}", packet.Id));
            switch (packet.Id)
            {
                case PacketType.LobbyInfo:
                    var info = packet as LobbyInfoPacket;
                    Debug.Log(info.ToString());
                    if (info.PlayerInfo.Any(p => !p.Ready))
                    {
                        var reply = new LobbyUpdatePacket();
                        reply.Ready = true;
                        client.Send(reply);
                    }
                    break;
                case PacketType.LoadGame:
                    var myId = (packet as LoadGamePacket).ClientId;
                    var ack = new LoadGameAckPacket();
                    ack.ClientId = myId;
                    client.Send(ack);
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
                    playerManager.OnEndGame(packet as EndGamePacket);
                    break;
                default:
                    // Debug.Log("Default");
                    Debug.Log(string.Format("Type: {0}, str: {1}", packet.Id, packet.ToString()));
                    break;
            }
        }
    }
}
