using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class DisplayUserId : MonoBehaviour
    {
        public Text userId = null;

        PhotonView pv = null;

        void Start()
        {
            pv = GetComponent<PhotonView>();
            if (pv.owner != null)
            {
                userId.text = pv.owner.NickName;
            }
        }        
    }
}
