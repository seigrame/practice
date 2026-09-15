using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using HTC.UnityPlugin.Vive;

namespace SocialVR
{
    public class VRMenuController : MonoBehaviour
    {
        public GameObject leftLaserPointer = null;
        public GameObject rightLaserPointer = null;

        void Start()
        {
            leftLaserPointer.SetActive(false);
            rightLaserPointer.SetActive(false);
        }

        void LateUpdate()
        {
            //if (ViveInput.GetPressUpEx(HandRole.LeftHand, ControllerButton.Menu))
            //{
            //    //Debug.Log("Left Hand Menu!!");

            //    bool isVisible = UIMainMenu.Instance.gameObject.activeSelf;
            //    UIMainMenu.Instance.ShowUI(isVisible == false);
            //}
            //else if (ViveInput.GetPressUpEx(HandRole.RightHand, ControllerButton.Menu))
            //{
            //    bool isActive = rightLaserPointer.activeSelf;
            //    rightLaserPointer.SetActive(!isActive);
            //}
        }
    }
}
