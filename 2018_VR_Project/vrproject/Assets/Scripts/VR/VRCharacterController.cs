using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class VRCharacterController : MonoBehaviour
    {
        public GameObject hmd = null;
        public GameObject character = null;
                
        void Update()
        {
            character.transform.rotation = Quaternion.Euler(0, hmd.transform.eulerAngles.y, 0);
            character.transform.position = hmd.transform.position;
        }
    }
}
