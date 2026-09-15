using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

namespace SocialVR
{
    /// <summary>
    /// StandardAssets 에 FirstPersonController 를 참조
    /// </summary>
    public class FirstPersonCamera : MonoBehaviour
    {
        public Camera cam;
        public MouseLook mouseLook = new MouseLook();

        Rigidbody _rigidBody;        
        
        public void Init()
        {
            _rigidBody = GetComponent<Rigidbody>();
            mouseLook.Init(transform, cam.transform);
        }

        void Update()
        {
            // 마우스커서가 안보일 경우만 카메라 업데이트처리
            if (Cursor.visible == false)
            {
                RotateView();
            }
            
            mouseLook.UpdateCursorLock();
        }        

        /// <summary>
        /// 카메라 회전처리
        /// </summary>
        void RotateView()
        {
            if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                return;

            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
            _rigidBody.velocity = velRotation * _rigidBody.velocity;
        }        
    }    
}
