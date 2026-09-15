using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    [RequireComponent(typeof(JellyMesh))]
    public class JellyMove : MonoBehaviour
    {
        public GameObject dummy = null;
        public LeanTweenPath path = null;
        public float jumpTimeMin = 2.0f;
        public float jumpTimeMax = 3.0f;
        public float jumpForce = 10.0f;
        public float moveTimeMin = 30.0f;
        public float moveTimeMax = 50.0f;

        JellyMesh _jellyMesh = null;

        float _elapsedTime = 0.0f;

        void Start()
        {
            _jellyMesh = GetComponent<JellyMesh>();
            Debug.Assert(_jellyMesh != null);

            _elapsedTime = Random.Range(jumpTimeMin, jumpTimeMax);
            
            CreateDummy();    
        }

        /// <summary>
        /// 더미 생성 및 이동처리
        /// </summary>
        void CreateDummy()
        {
            float moveTime = Random.Range(moveTimeMin, moveTimeMax);

            LTSpline s = new LTSpline(path.splineVector());
            LeanTween.move(dummy, s, moveTime).setOrientToPath(true).setRepeat(-1).setLoopPingPong();
        }

        void FixedUpdate()
        {
            _elapsedTime -= Time.deltaTime;

            Vector3 targetPos = _jellyMesh.transform.position;

            if (_elapsedTime < 0.0f)
            {
                // 점프처리
                _jellyMesh.AddForce(Vector3.up * jumpForce, false, ForceMode.Impulse);

                _elapsedTime = Random.Range(jumpTimeMin, jumpTimeMax);
            }

            // 더미의 x, z 좌표를 타겟에 적용
            _jellyMesh.SetPosition(new Vector3(dummy.transform.position.x, targetPos.y, dummy.transform.position.z), false);
        }
    }
}
