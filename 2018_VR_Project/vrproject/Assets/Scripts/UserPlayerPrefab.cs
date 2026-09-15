using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class UserPlayerPrefab
    {
        private readonly string CHARACTER_NICKNAME = "CHARACTER_NICKNAME";
        private readonly string CHARACTER_INDEX = "CHARACTER_INDEX";

        public string GetCharacterNickName()
        {
            string nickName = PlayerPrefs.GetString(CHARACTER_NICKNAME, "");
            if (string.IsNullOrEmpty(nickName) == false)
            {
                return nickName;
            }

            nickName = "Guest" + Random.Range(1, 9999);
            PlayerPrefs.SetString(CHARACTER_NICKNAME, nickName);

            return nickName;
        }

        public int GetCharacterIndex()
        {
            if (PlayerPrefs.HasKey(CHARACTER_INDEX))
            {
                return PlayerPrefs.GetInt(CHARACTER_INDEX);
            }

            int index = 0;
            SetCharacterIndex(index);

            return index;
        }

        public void SetCharacterIndex(int index)
        {
            PlayerPrefs.SetInt(CHARACTER_INDEX, index);
        }
    }
}
