using UnityEngine;

public class CurrentRoomCanvas : MonoBehaviour {

    public void OnClickStartSync()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        //하나의 클라이언트가 룸내 모든 클라이언트들이 로드해야할 레벨을 정의
        PhotonNetwork.LoadLevel(2);
    }    
}
