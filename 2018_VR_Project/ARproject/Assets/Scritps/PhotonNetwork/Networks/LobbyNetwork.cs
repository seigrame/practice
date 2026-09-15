using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetwork : MonoBehaviour {

    private void Start()
    {
        print("Connecting to Server...");
        PhotonNetwork.ConnectUsingSettings("0.0.0"); //포톤네트워크 게임 버전 체크(버전이름)?

        //방에 참여중이면 룸목록을 바로 보여줌
        if (PhotonNetwork.inRoom)
        {
            MainCanvasManager.Instance.CurrentCanvas.transform.SetAsLastSibling();
        }
    }

    void OnConnectedToMaster()
    {
        print("Connected to Master");
        PhotonNetwork.automaticallySyncScene =true; //모든 클라이언트가 방장에게 싱크를 맞춤.
        //플레이어네트워크 스크립트에 저장된 이름으로 포톤네트워크 접속
        PhotonNetwork.playerName = PlayerNetwork.Instance.PlayerName;
        //기본타입의 로비에 참여
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    //로비로 돌아감
    void OnJoinedLobby()
    {
        print("Joined Lobby.");

        if (!PhotonNetwork.inRoom) //방에 참여중이 아니라면
        {
            MainCanvasManager.Instance.LobbyCanvas.transform.SetAsLastSibling(); //하이어라키의 뒤쪽으로 이동
        }
    }

}
