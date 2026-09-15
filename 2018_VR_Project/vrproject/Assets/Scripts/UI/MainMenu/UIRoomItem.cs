using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class UIRoomItem : MonoBehaviour
    {
        [HideInInspector] public string roomName = "";
        [HideInInspector] public int connectPlayer = 0;
        [HideInInspector] public int maxPlayers = 0;

        public Text textRoomName = null;

        public void DespRoomData()
        {
            textRoomName.text = string.Format("{0}\n({1}/{2})", roomName, connectPlayer.ToString(), maxPlayers.ToString());
        }
    }
}
