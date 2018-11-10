using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestScript : MonoBehaviour
{

    private NetworkClient client;
    private void OnDestroy()
    {
        client.Dispose();
    }
    // Use this for initialization
    void Start()
    {
        client = new NetworkClient();
        client.Connect("localhost", 2050);

        client.OnPacket += (packet) =>
        {
            Debug.Log(string.Format("Received Server Packet {0}", packet.Id));
            switch (packet.Id)
            {
                case PacketType.LobbyInfo:
                    var info = packet as Packets.LobbyInfoPacket;
                    Debug.Log(info.ToString());
                    if (info.PlayerInfo.Any(p => !p.Ready))
                    {
                        var reply = new Packets.LobbyUpdatePacket();
                        reply.Ready = true;
                        client.Send(reply);
                    }
                    break;
                case PacketType.LoadGame:
                    var myId = (packet as Packets.LoadGamePacket).ClientId;
                    var ack = new Packets.LoadGameAckPacket();
                    ack.ClientId = myId;
                    client.Send(ack);
                    break;
                default:
                    Debug.Log(string.Format("Type: {0}, str: {1}", packet.Id, packet.ToString()));
                    break;
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

    // Update is called once per frame
    void Update()
    {

    }
}
