using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SocialVR
{
    public class LobbyNetwork : Photon.MonoBehaviour
    {
        public GameObject roomItem = null;

        GameObject _scrollContents = null;

        GUIStyle _guiStyle = new GUIStyle();

        void Awake()
        {
            if (PhotonNetwork.connected == false)
            {
                PhotonNetwork.ConnectUsingSettings("v1.0");
            }

            if (String.IsNullOrEmpty(PhotonNetwork.playerName))
            {
                PhotonNetwork.playerName = GameManager.Instance.UserPlayerPrefab.GetCharacterNickName();
            }

            _guiStyle.fontSize = 30;
            _guiStyle.normal.textColor = Color.white;
        }

        void OnEnable()
        {
            EventManager.onCreateRoomEvent += CreateRoom;
        }

        void OnDisable()
        {
            EventManager.onCreateRoomEvent -= CreateRoom;            
        }

        /// <summary>
        /// 로비 접속 시 호출
        /// </summary>
        void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby");            
        }

        /// <summary>
        /// 로비를 떠날 경우 호출됨. 룸으로 입장 시 호출됨
        /// </summary>
        void OnLeftLobby()
        {
            Debug.Log("OnLeftLobby");
        }

        void OnJoinedRoom()
        {
            RoomType roomType = (RoomType)PhotonNetwork.room.CustomProperties["RoomType"];
            Debug.Log("OnJoinedRoom : " + roomType.ToString());

            LoadGameRoom(roomType);
        }        

        void OnCreateRoom()
        {
            // 게임룸 또는 미팅룸으로 이동
            //RoomType roomType = (RoomType)PhotonNetwork.room.CustomProperties["RoomType"];
            //Debug.Log("OnCreateRoom : " + roomType.ToString());
        }

        void LoadGameRoom(RoomType roomType)
        {
            string sceneName = string.Empty;
            switch (roomType)
            {
                case RoomType.Meeting:
                    sceneName = "MeetingRoom";
                    break;
                case RoomType.MultiGame:
                    sceneName = "MultiGameRoom";
                    break;
            }

            Debug.Assert(sceneName != string.Empty);

            StartCoroutine(SceneLoader.Instance.LoadSceneAsync(sceneName));                       
        }

        void OnPhotonRandomJoinFailed()
        {
            Debug.Log("OnPhotonRandomJoinFailed");            
        }

        void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("Create Room Failed = " + codeAndMsg[1]);
        }

        /// <summary>
        /// 마스터로 접속했을 때 호출
        /// </summary>
        void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster : " + PhotonNetwork.connectionStateDetailed);

            PhotonNetwork.JoinLobby();            
        }        

        /// <summary>
        /// 생성된 룸 목록이 변경됐을 때 호출되는 콜백함수
        /// </summary>
        void OnReceivedRoomListUpdate()
        {
            if (_scrollContents == null)
            {
                _scrollContents = UIMainMenu.Instance.createRoomListMenu.scrollContent;
            }

            RoomInfo[] roomInfos = PhotonNetwork.GetRoomList();

            Debug.Log("OnReceivedRoomListUpdate : " + roomInfos.Length);

            foreach (RoomInfo _room in roomInfos)
            {
                Debug.Log( _room.Name);

                // 미팅룸은 리스트에 포함시키지 않음
                if (Constants.MeetingRoomName == _room.Name)
                {
                    continue;
                }

                GameObject room = (GameObject)Instantiate(roomItem);

                room.transform.SetParent(_scrollContents.transform, false);

                UIRoomItem roomitem = room.GetComponent<UIRoomItem>();
                roomitem.roomName = _room.Name;
                roomitem.connectPlayer = _room.PlayerCount;
                roomitem.maxPlayers = _room.MaxPlayers;

                roomitem.DespRoomData();

                // 버튼이벤트 연결
                roomitem.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(roomitem.roomName); });                
            }
        }

        /// <summary>
        /// RoomItem 클릭 시 호출
        /// </summary>
        /// <param name="roomName"></param>
        void OnClickRoomItem(string roomName)
        {
            // 룸에 입장
            PhotonNetwork.JoinRoom(roomName);
        }

        void CreateRoom(RoomType roomType)
        {
            string roomName = string.Empty;
            switch (roomType)
            {
                case RoomType.Meeting:
                    roomName = Constants.MeetingRoomName;
                    break;
                case RoomType.MultiGame:
                    {
                        roomName = "Game_" + Random.Range(0, 999).ToString("000");
                    }
                    break;
            }

            ExitGames.Client.Photon.Hashtable customProps = new ExitGames.Client.Photon.Hashtable();
            customProps["RoomType"] = roomType;

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = Constants.MaxRoomPlayers;
            roomOptions.CustomRoomProperties = customProps;

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        void OnGUI()
        {
            // 화면 좌측 상단에 접속상태 표시            
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), _guiStyle);   
        }
    }
}
