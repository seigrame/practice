using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace SocialVR
{
    public class Player : Singleton<Player>
    {
        public Transform[] hmdTransforms;

        public GameObject rigSteamVR;
        public GameObject rig2DFallback;

        void OnEnable()
        {
            if (SteamVR.instance != null)
            {
                ActivateRig(rigSteamVR);
            }
            else
            {
                ActivateRig(rig2DFallback);
            }
        }

        void ActivateRig(GameObject rig)
        {
            if(rigSteamVR != null) rigSteamVR.SetActive(rig == rigSteamVR);
            if(rig2DFallback != null) rig2DFallback.SetActive(rig == rig2DFallback);
        }
    }
}
