using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    /// <summary>
    /// FallbackCameraController 참조
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public float minimumY = -60f;
        public float maximumY = 60f;

        private Vector3 startEulerAngles;
        private Vector3 startMousePosition;
        private float realTime;

        void Update()
        {
            // 카메라 회전처리
            Vector3 mousePosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(1) /* right mouse */)
            {
                startMousePosition = mousePosition;
                startEulerAngles = transform.localEulerAngles;
            }

            if (Input.GetMouseButton(1) /* right mouse */)
            {
                Vector3 offset = mousePosition - startMousePosition;

                float rotationX = Mathf.Clamp(-offset.y * 360.0f / Screen.height, minimumY, maximumY);
                float rotationY = offset.x * 360.0f / Screen.width;
                transform.localEulerAngles = startEulerAngles + new Vector3(rotationX, rotationY, 0.0f);
            }
        }
    }
}
