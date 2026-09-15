using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class CharacterController : MonoBehaviour
    {
        public Rigidbody target = null;
        public float speed = 1.0f;
        public float turnSpeed = 2.0f;
        public LayerMask groundLayers = -1;
        public float groundedCheckOffset = 0.0f;
        public bool showGizmos = true;

        private Animator _anim = null;

        private const float _inputThreshold = 0.01f;
        private const float _groundDrag = 5.0f;     // 저항력
        private const float _groundedDistance = 0.5f;

        private bool _grounded;
        public bool Grounded { get { return _grounded; } }

        private bool _isRemotePlayer = true;

        void Start()
        {
            if (target == null)
            {
                target = GetComponent<Rigidbody>();
            }

            _anim = GetComponent<Animator>();

            // 캐릭터가 넘어지지 않게 해줌
            target.freezeRotation = true;  
        }

        void Update()
        {
            if (_isRemotePlayer == true)
                return;

            // 키보드 좌우키 입력 시 캐릭터 회전처리 (마우스회전처리가 되지 않는 상태에서만 회전됨)
            float rotationAmount = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
            target.transform.RotateAround(target.transform.up, rotationAmount);            
        }

        void FixedUpdate()
        {
            _grounded = Physics.Raycast(
                target.transform.position + target.transform.up * -groundedCheckOffset,
                target.transform.up * -1,
                _groundedDistance,
                groundLayers
            );

            if (_isRemotePlayer == true)
                return;

            if (_grounded == true)
            {
                target.drag = _groundDrag;

                // 키보드 위아래키 입력 시 캐릭터 이동
                float inputV = Input.GetAxis("Vertical");
                Vector3 movement = inputV * target.transform.forward;

                if (movement.magnitude > _inputThreshold)
                {
                    // 이동처리
                    target.AddForce(movement.normalized * speed, ForceMode.VelocityChange);

                    // 애니메이션 처리
                    _anim.SetFloat("inputV", inputV);
                }
                else
                {
                    target.velocity = new Vector3(0.0f, target.velocity.y, 0.0f);
                    return;
                }
            }
            else
            {
                target.drag = 0.0f;
            }
        }

        public void SetIsRemotePlayer(bool val)
        {
            _isRemotePlayer = val;
        }

        void OnDrawGizmos()
        // Use gizmos to gain information about the state of your setup
        {
            if (!showGizmos || target == null)
            {
                return;
            }

            Gizmos.color = _grounded ? Color.blue : Color.red;
            Gizmos.DrawLine(target.transform.position + target.transform.up * -groundedCheckOffset,
                target.transform.position + target.transform.up * -(groundedCheckOffset + _groundedDistance));
        }
    }
}
