using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class UIRoomMenu : MonoBehaviour
    {
        [SerializeField] Button _meetingRoomButton = null;
        [SerializeField] Button _singleRoomButton = null;
        [SerializeField] Button _gameRoomButton = null;

        [SerializeField] GameObject _roomListPanel = null;        

        void OnEnable()
        {
            gameObject.SetActive(true);
            _roomListPanel.SetActive(false);

            _meetingRoomButton.onClick.AddListener(OnMeetingRoomButtonClicked);
            _singleRoomButton.onClick.AddListener(OnSingleRoomButtonClicked);
            _gameRoomButton.onClick.AddListener(OnGameRoomButtonClicked);
        }

        void OnDisable()
        {
            _meetingRoomButton.onClick.RemoveListener(OnMeetingRoomButtonClicked);
            _gameRoomButton.onClick.RemoveListener(OnGameRoomButtonClicked);
        }

        /// <summary>
        /// 미팅룸 버튼 클릭 시
        /// </summary>
        void OnMeetingRoomButtonClicked()
        {
            EventManager.OnCreateRoom(RoomType.Meeting);
        }

        /// <summary>
        /// 싱글룸 버튼 클릭 시
        /// </summary>
        void OnSingleRoomButtonClicked()
        {
            // 싱글 게임룸으로 이동
            SceneLoader.Instance.LoadScene("SingleGameRoom");            
        }

        /// <summary>
        /// 게임룸 버튼 클릭 시
        /// </summary>
        void OnGameRoomButtonClicked()
        {
            gameObject.SetActive(false);
            _roomListPanel.SetActive(true);
        }
    }
}
