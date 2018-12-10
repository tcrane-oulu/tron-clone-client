using Packets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    int ownId;
    int endGameTarget = -1;
    Dictionary<int, GameObject> players;
    public GameObject playerPrefab;
    public GameObject opponentPrefab;
    public GameObject deathParticles;
    public GameObject victoryImage;
    Player netManager;

    // Use this for initialization
    void Start()
    {
        players = new Dictionary<int, GameObject>();
        netManager = FindObjectOfType<Player>();
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
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                var newDir = Direction.Left;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                var newDir = Direction.Down;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                var newDir = Direction.Right;
                var playerUpdate = new PlayerUpdatePacket(newDir);
                netManager.Send(playerUpdate);
            }
        }
        if (endGameTarget != -1)
        {
            victoryImage.SetActive(true);
            var pos = players[endGameTarget].transform.position;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(pos.x, pos.y, Camera.main.transform.position.z), 0.02f);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 40, 0.02f);
        }
    }

    public void OnLoadGame(LoadGamePacket loadGame)
    {
        ownId = loadGame.ClientId;
    }

    public void OnDeath(DeathPacket death)
    {
        var playerPos = players[death.PlayerId].transform.position;
        var deathEffect = Instantiate(deathParticles, playerPos, Quaternion.identity);
    }

    public void OnEndGame(EndGamePacket endGame)
    {
        endGameTarget = endGame.WinnerId;
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
