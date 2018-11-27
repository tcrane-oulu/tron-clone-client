using Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    int ownId;
    Dictionary<int, GameObject> players;
    public GameObject playerPrefab;
    public GameObject opponentPrefab;
    TestScript netManager;

    // Use this for initialization
    void Start()
    {
        players = new Dictionary<int, GameObject>();
        netManager = FindObjectOfType<TestScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ownId != 0)
        {
            // if we are in the game, we can start processing input.
            if (Input.GetKeyDown(KeyCode.W))
            {
                var newDir = Direction.Up;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
                Debug.Log("MOVING UP");
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                var newDir = Direction.Left;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
                Debug.Log("MOVING LEFT");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                var newDir = Direction.Down;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
                Debug.Log("MOVING DOWN");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                var newDir = Direction.Right;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
                Debug.Log("MOVING RIGHT");
            }
        }
    }

    public void OnLoadGame(LoadGamePacket loadGame)
    {
        ownId = loadGame.ClientId;
    }

    public void OnTick(TickPacket tick)
    {
        foreach (var info in tick.Players)
        {
            if (!players.ContainsKey(info.Id))
            {
                // instantiate.
                Vector3 pos = info.Position;
                if (info.Id == ownId)
                {
                    var newPlayer = Instantiate(playerPrefab, pos, Quaternion.identity);
                    players.Add(info.Id, newPlayer);
                    // parent the camera to this object.
                    // Camera.main.transform.SetParent(newPlayer.transform);
                    // Camera.main.transform.localPosition = new Vector3(0, 0, -10);
                }
                else
                {
                    var newOpponent = Instantiate(opponentPrefab, pos, Quaternion.identity);
                    players.Add(info.Id, newOpponent);
                }
            }
            else
            {
                var player = players[info.Id];
                player.transform.position = Vector3.Lerp(player.transform.position, info.Position, 0.8f);
            }
        }
    }
}
