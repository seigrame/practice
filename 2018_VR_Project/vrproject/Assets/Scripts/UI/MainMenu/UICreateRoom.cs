using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class UICreateRoom : MonoBehaviour
    {
        public GameObject scrollContent = null;
        public Button createRoomButton = null;

        void OnEnable()
        {
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);
        }

        void OnDisable()
        {
            createRoomButton.onClick.RemoveListener(OnCreateRoomButtonClicked);
        }

        /// <summary>
        /// 방 생성 버튼 클릭 시
        /// </summary>
        void OnCreateRoomButtonClicked()
        {
            // 멀티대전 방 생성
            EventManager.OnCreateRoom(RoomType.MultiGame);
        }        
    }
}
