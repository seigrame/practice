using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayoutGroup : MonoBehaviour {

    [SerializeField]
    GameObject _playerListingPrefab;
    GameObject PlayerListingPrefab { get { return _playerListingPrefab; } }

    List<PlayerListing> _playerListings = new List<PlayerListing>();
    List<PlayerListing> PlayerListings
    {
        get { return _playerListings; }
    }

    private void Start()
    {
        if (!PhotonNetwork.inRoom)
            return;

        // 게임플레이가 끝나고 로비로 다시 들어왔을 경우 플레이어목록 갱신처리
        if(PhotonNetwork.playerList.Length > 0)
        {
            for (int i = 0; i < PhotonNetwork.playerList.Length; ++i)
            {
                PlayerJoinedRoom(PhotonNetwork.playerList[i]);
            }
        }
    }

    //방을 폭파시킴 - > 마스터 클라이언트가 방을 떠날때마다 호출
    void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {   
        PhotonNetwork.LeaveRoom();
    }
    //방에 참여할때마다 호출
    void OnJoinedRoom()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        MainCanvasManager.Instance.CurrentCanvas.transform.SetAsLastSibling();// SetAsLastSibling => 하이어라키의 가장 마지막으로 순서 변경
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;                //
        for(int i=0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }
    //다른 플레이어가 접속하면 연결상태를 업데이트해줌
    void OnPhotonPlayerConnected(PhotonPlayer photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }

    //방에서 나갈때마다 호출
    void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLetfRoom(photonPlayer);
    }
    //플레이어가 방에 참여함 
    void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer == null)   //플레이어가 없다면 리턴
        { return; }

        PlayerLetfRoom(photonPlayer);       //플레이어가 방을 떠났다면 아이디를 제외

        GameObject playerListingObj = Instantiate(PlayerListingPrefab);     //플레이어를 가리키는 프리팹 생성
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);                      //해당 플레이어의 닉네임을 적용함

        PlayerListings.Add(playerListing);                                  //리스트에 해당 플레이어를 추가함
    }
    //플레이어가 방을 떠남
    void PlayerLetfRoom(PhotonPlayer photonPlayer)
    {   //리스트에서 인덱스를 비교해 플레이어를 찾음
        int index = PlayerListings.FindIndex(item => item.PhotonPlayer == photonPlayer);
        //플레이어가 없다면 
        if(index != -1)
        {   //게임오브젝트를 삭제
            Destroy(PlayerListings[index].gameObject);  //플레이어를 호칭하는 게임오브젝트를 제거함
            PlayerListings.RemoveAt(index);             //인덱스를 제거함.
        }
    }

    public void OnClickRoomState()
    {
        if (!PhotonNetwork.isMasterClient)  //방장이 아니라면 리턴
            return;
        PhotonNetwork.room.IsOpen = !PhotonNetwork.room.IsOpen;     //클릭으로 방을 열고 닫을수 있음
        PhotonNetwork.room.IsVisible = PhotonNetwork.room.IsOpen;   //방이 열리면 로비에 공개됨
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
