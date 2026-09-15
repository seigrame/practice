using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance;
    [SerializeField]
    LobbyCanvas _lobbyCanvas;
    public LobbyCanvas LobbyCanvas { get { return _lobbyCanvas; } }

    [SerializeField]
    CurrentRoomCanvas _currentCanvas;
    public CurrentRoomCanvas CurrentCanvas
    {
        get { return _currentCanvas; }
    }

    void Awake()
    {
        Instance = this;
    }
   
}
