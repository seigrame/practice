using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class UIAvatarItem : MonoBehaviour
    {
        public Text characerName = null;

        [HideInInspector]
        public Image iconImage = null;

        void Start()
        {
            iconImage = GetComponent<Image>();
        }
    }
}
