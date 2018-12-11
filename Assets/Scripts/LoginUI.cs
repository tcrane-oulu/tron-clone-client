using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    const int VERSION = 6520;
    public GameObject loginPanel;
    public Text username;
    public Button loginButton;

    public GameObject lobbyPanel;
    public GameObject playerPanel;
    public GameObject playerPrefab;
    public Text statusText;
    public GameObject winnerPanel;
    public Text winnerText;

    public Text serverIpText;
    public Text portText;
    public GameObject border;
    public GameObject lobbyButton;

    private bool ready;

    private Player netManager;

    private Queue<Action> actionQueue;

    private PlayerManager playerManager;
    private object mutex;

    // Use this for initialization
    void Start()
    {
        actionQueue = new Queue<Action>();
        mutex = new object();
        ready = false;
        netManager = FindObjectOfType<Player>();
        playerManager = FindObjectOfType<PlayerManager>();
        EnableAll();
    }

    public void EnableAll()
    {
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        winnerPanel.SetActive(false);
        lobbyButton.SetActive(false);
        border.SetActive(false);
        loginButton.interactable = true;
    }

    public void OnLoginClick()
    {
        loginButton.interactable = false;
        netManager.Login(username.text, VERSION, serverIpText.text, int.Parse(portText.text));
        netManager.client.Connected += (connected) =>
        {
            if (connected)
            {
                lock (mutex)
                {
                    actionQueue.Enqueue(new Action(() =>
                    {
                        loginPanel.SetActive(false);
                        lobbyPanel.SetActive(true);
                    }));
                }
            }
        };
    }

    public void OnReadyClick()
    {
        ready = !ready;
        var reply = new Packets.LobbyUpdatePacket();
        reply.Ready = ready;
        netManager.client.Send(reply);
    }

    public void OnStartGame(Packets.StartGamePacket startGame)
    {
        lock (mutex)
        {
            actionQueue.Enqueue(new Action(() =>
            {
                statusText.text = "Got all responses!";
                DisableAll();
            }));
        }
    }

    public void DisableAll()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        winnerPanel.SetActive(false);
        lobbyButton.SetActive(false);
        border.SetActive(true);
    }

    public void OnLobbyClick()
    {
        playerManager.Cleanup();
        EnableAll();
    }

    public void OnEndGame(Packets.EndGamePacket endGame)
    {
        winnerText.text = endGame.WinnerName;
        winnerPanel.SetActive(true);
        lobbyButton.SetActive(true);
    }

    public void OnLobbyInfo(Packets.LobbyInfoPacket lobbyInfo)
    {
        // clear the existing children.
        foreach (Transform child in playerPanel.transform)
        {
            Destroy(child.gameObject);
        }
        var readyCount = 0;
        foreach (var info in lobbyInfo.PlayerInfo)
        {
            GameObject player = Instantiate(playerPrefab, playerPanel.transform);
            player.GetComponent<PlayerPanelController>().SetProps(info.Name, info.Ready);
            if (info.Ready)
            {
                readyCount++;
            }
        }
        statusText.text = string.Format("{0}/{1} players ready.", readyCount, lobbyInfo.PlayerInfo.Length);
    }

    public void OnLoadGame()
    {
        statusText.text = "Waiting for player responses...";
    }

    // Update is called once per frame
    void Update()
    {
        lock (mutex)
        {
            if (actionQueue.Count > 0)
            {
                actionQueue.Dequeue().Invoke();
            }
        }
    }
}
