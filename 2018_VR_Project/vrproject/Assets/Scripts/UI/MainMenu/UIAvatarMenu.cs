using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class UIAvatarMenu : MonoBehaviour
    {
        public UIAvatarList avatarList = null;

        public void Init()
        {
            avatarList.Init();
        }
    }
}
