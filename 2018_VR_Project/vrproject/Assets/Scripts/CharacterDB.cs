using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class CharacterDB : MonoBehaviour
    {
        public List<GameObject> characterList = new List<GameObject>();

        public static List<GameObject> Load()
        {
            GameObject go = (GameObject)Resources.Load("CharacterDB");
            if (go != null)
            {
                return go.GetComponent<CharacterDB>().characterList;
            }

            return null;
        }
    }
}
