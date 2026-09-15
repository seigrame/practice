using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class BillboardCanvas : MonoBehaviour
    {
        Transform _thisT;
        Transform _camT;

        void Start()
        {
            _thisT = this.transform;

            _camT = Camera.main.transform;
        }

        void Update()
        {
            _thisT.LookAt(_camT);
        }
    }
}
