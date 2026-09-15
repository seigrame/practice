using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SocialVR
{
    public class CharacterNetwork : Photon.MonoBehaviour
    {
        // 로비 등에서 3인칭시점으로 서있는 캐릭터의 경우 해당 변수를 true 로 설정
        // 캐릭터를 컨트롤할 수 없는 상태
        public bool isThirdPersonCharacter = false;

        CharacterController _controllerScript;
        FirstPersonCamera _cameraScript;

        bool _appliedInitialUpdate;

        Vector3 _correctPlayerPos = Vector3.zero;
        Quaternion _correctPlayerRot = Quaternion.identity;

        void Awake()
        {
            _controllerScript = GetComponent<CharacterController>();
            _cameraScript = GetComponent<FirstPersonCamera>();
        }

        void Start()
        {
            if (PhotonNetwork.connected == true && PhotonNetwork.inRoom)
            {
                if (photonView.isMine)
                {
                    SetMyCharacter();
                }
                else
                {
                    _cameraScript.enabled = false;
                }

                _controllerScript.SetIsRemotePlayer(!photonView.isMine);                
            }
            else
            {
                SetMyCharacter();

                _controllerScript.SetIsRemotePlayer(false);
            }
                       
            StringBuilder sb = new StringBuilder();
            sb.Append(gameObject.name);
            
            if (PhotonNetwork.connected == true)
            {
                sb.Append("_");
                sb.Append(photonView.viewID);
            }                

            gameObject.name = sb.ToString();
        }

        void Update()
        {
            // 캐릭터 움직임 보간
            if (photonView.isMine == false && PhotonNetwork.inRoom)
            {
                transform.position = Vector3.Lerp(transform.position, _correctPlayerPos, Time.deltaTime * 5);
                transform.rotation = Quaternion.Lerp(transform.rotation, _correctPlayerRot, Time.deltaTime * 5);
            }
        }

        void SetMyCharacter()
        {
            if (SteamVR.instance == null) // SteamVR 을 사용하지 않는 경우
            {
                if (isThirdPersonCharacter == false)
                {
                    _cameraScript.enabled = true;
                    _cameraScript.cam = Camera.main;
                    _cameraScript.cam.transform.parent = transform;
                    _cameraScript.cam.transform.localPosition = new Vector3(0, 1, 0.2f);
                    _cameraScript.cam.transform.localEulerAngles = new Vector3(0, 0, 0);
                    _cameraScript.Init();
                }
                else
                {
                    GetComponent<CharacterController>().enabled = false;
                    GetComponent<FirstPersonCamera>().enabled = false;
                }
            }
            else
            {
                // SteamVR 설정
                _cameraScript.enabled = false;
                _controllerScript.enabled = false;
                transform.parent = Player.Instance.rigSteamVR.transform.Find("[CameraRig]");
                
            }

            //UIMainMenu.Instance.transform.parent = _cameraScript.cam.transform;
            //UIMainMenu.Instance.transform.localPosition = new Vector3(0, 0, 1.5f);
            //UIMainMenu.Instance.gameObject.SetActive(false);
        }

        void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // 다른 플레이어들에게 내 캐릭터에 대한 데이타전송
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(GetComponent<Rigidbody>().velocity);
            }
            else
            {
                // 네트워크 플레이어일 경우 데이타를 받아서 저장
                _correctPlayerPos = (Vector3)stream.ReceiveNext();
                _correctPlayerRot = (Quaternion)stream.ReceiveNext();
                GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();

                if (_appliedInitialUpdate == false)
                {
                    _appliedInitialUpdate = true;
                    transform.position = _correctPlayerPos;
                    transform.rotation = _correctPlayerRot;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
            }
        }        
    }
}
