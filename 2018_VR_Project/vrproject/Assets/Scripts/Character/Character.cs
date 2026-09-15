using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class Character : MonoBehaviour
    {
        public CharacterInfo info = new CharacterInfo();        
    }

    [System.Serializable]
    public class CharacterInfo
    {
        public Sprite image;
    }
}
