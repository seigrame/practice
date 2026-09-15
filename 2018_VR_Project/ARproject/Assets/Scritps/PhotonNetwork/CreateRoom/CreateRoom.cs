using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {

    [SerializeField]
    Text _roomName;
    Text Roomname
    {
        get { return _roomName; }
    }
    //클릭으로 방을 생성함
    public void OnClick_CreateRoom()
    {   //방의 옵션 초기화(로비에 방이 드러남, 방에 참여가능, 최대플레이어수는 4명)
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };
        //텍스트를 받아 방을 만들고, 만약 같은 이름의 방이 존재한다면 생성에 실패함.
        if(PhotonNetwork.CreateRoom(Roomname.text, roomOptions, TypedLobby.Default))
        {
            print("create room successfully sent.");
        }
        else
        {
            print("create room failed to send.");
        }
    }
    //방을 생성하는데 실패했다면 
    void OnPhotonCreateRoomFailed(object[] codeAndMessage)
    {   //에러 메세지 전송
        print("create room failed :" + codeAndMessage[1]);
    }
    //방이 만들어졌다면
    void OnCreatedRoom()
    {   //성공적으로 만들어졌다는 메세지를 띄움
        print("Room created successfully.");
    }
}
