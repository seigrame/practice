using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class Title : MonoBehaviour
    {
        public Button startButton = null;

        void Start()
        {
            startButton.onClick.AddListener(OnStart);
        }

        void OnStart()
        {
            SceneLoader.Instance.LoadScene("Lobby");            
        }
    }
}
