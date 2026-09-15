using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLayoutGroup : MonoBehaviour
{
    [SerializeField]
    GameObject _roomListingPrefab; //참조할 프리팹
    GameObject RoomListingPrefab { get { return _roomListingPrefab; } } // 그 프리팹은 불러오기만 가능

    List<RoomListing> _roomListingButton = new List<RoomListing>(); //생성될 방참여 버튼의 리스트를 만듬
    List<RoomListing> RoomListingButtons {  get { return _roomListingButton; } }

    //방 목록 업데이트 함수
    void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList(); // 포톤에서 방 목록을 얻어옴
        //포톤에서 얻어온 모든 방 목록을 검사해서
        foreach(RoomInfo room in rooms)
        {   //방에 대한 정보를 줌
            RoomReceived(room);
        }
        RemoveOldRooms();//오래된 방은 제거함
    }
    //방에 대한 정보를 주는 함수
    void RoomReceived(RoomInfo room)
    {   //방 목록의 인덱스를 찾아 존재하는지 검색
        int index = RoomListingButtons.FindIndex(item => item.RoomName == room.Name);
        //존재하지않는 상황에서
        if(index == -1)
        {   //방이 보이는 상태이고, 리스트에 여유 공간이 있다면
            if(room.IsVisible && room.playerCount < room.MaxPlayers)  //room.IsVisible => 로비에 방을 나열할지 여부를 정의합니다. 방을 보이지 않게 만들거나 보이지 않게 변경할 수 있습니다.
            {   //프리팹으로 새로운 방 생성 => (포트리스를 생각하면 됨)
                GameObject roomListingObj = Instantiate(RoomListingPrefab);
                roomListingObj.transform.SetParent(transform, false); //생성된 오브젝트를 자식으로 붙이고,로컬 포지션에 위치하게합니다

                RoomListing roomListing = roomListingObj.GetComponent<RoomListing>();
                RoomListingButtons.Add(roomListing); //RoomListing 컴포넌트를 포함하고 있는 오브젝트를 리스트에 더함

                index = (RoomListingButtons.Count - 1);
            }
        }
        //리스트 상에 존재한다면
        if(index != -1)
        {   //해당 방의 인덱스를 알려줌
            RoomListing roomListing = RoomListingButtons[index];
            roomListing.SetRoomNameText(room.Name);                 //방의 이름을 변경 
            roomListing.Updated = true;                             //정보 갱신함
        }
    }
    //방을 제거하는 함수
    void RemoveOldRooms()
    {
        List<RoomListing> removeRooms = new List<RoomListing>();
        //
        foreach(RoomListing roomListing in RoomListingButtons) //로비에 등록된 방들을 검색해서
        {   
            if (!roomListing.Updated)           //로비에 공개되지 않은 방이 업데이트 된다면,
                removeRooms.Add(roomListing);   //해당 방을 로비 제거목록에 올림.
            else
                roomListing.Updated = false;    //로비에 공개된 방이 업데이트가 된다면, 업데이트를 하지않음
        }
        foreach(RoomListing roomListing in removeRooms) //제거 목록에 있는 방들을 검색해서 
        {
            GameObject roomListingObj = roomListing.gameObject;
            RoomListingButtons.Remove(roomListing);     //해당 목록들을 로비에서 제거함
            Destroy(roomListingObj);
        }
    }
}
