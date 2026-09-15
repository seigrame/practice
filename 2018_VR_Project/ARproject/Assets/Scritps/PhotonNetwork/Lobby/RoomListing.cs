using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour {

    [SerializeField]
    Text _roomNameText;
    Text RoomnameText
    {
        get { return _roomNameText; }
    }

    public string RoomName { get; private set; }
	public bool Updated { get; set; }

    private void Start()
    {
        GameObject lobbyCanvasObj = MainCanvasManager.Instance.LobbyCanvas.gameObject; // 로비 캔버스 연결
        if (lobbyCanvasObj == null)
            return; //예외처리

        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>(); //

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClickJoinRoom(RoomnameText.text)); // 버튼 클릭시 함수이벤트 추가
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners(); //함수이벤트 초기화
    }
    //방의 제목을 설정
    public void SetRoomNameText(string text)
    {
        RoomName = text;
        RoomnameText.text = RoomName;
    }
}
