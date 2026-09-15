using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class EventManager : MonoBehaviour
    {
        /// <summary>
        /// 방생성 이벤트 콜백함수
        /// </summary>
        public delegate void CreateRoomHandler(RoomType roomType);
        public static event CreateRoomHandler onCreateRoomEvent;
        public static void OnCreateRoom(RoomType roomType) { if (onCreateRoomEvent != null) onCreateRoomEvent(roomType); }
    }
}
