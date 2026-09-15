using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCanvas : MonoBehaviour {

    [SerializeField]
    RoomLayoutGroup _roomLayoutGroup;
    RoomLayoutGroup RoomLayoutGroup
    {
        get { return _roomLayoutGroup; }
    }

    public void OnClickJoinRoom(string roomName)
    {
        if(PhotonNetwork.JoinRoom(roomName))
        {

        }
        else
        {
            print("Join Room Failed");
        }
    }
}
