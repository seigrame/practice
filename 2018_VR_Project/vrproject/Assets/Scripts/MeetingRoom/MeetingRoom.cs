using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class MeetingRoom : Photon.MonoBehaviour
    {
        [SerializeField]
        Transform _spawnPoint = null;

        public SoundSource bgm = null;

        void Awake()
        {
            CharacterManager.Instance.Init();
            UIMainMenu.Instance.Init();

            CreateCharacter();

            // 네트워크 메세지 수신을 다시 연결
            PhotonNetwork.isMessageQueueRunning = true;

            // 배경사운드재생
            SoundManager.Instance.PlayBGM(bgm.name);
        }

        void CreateCharacter()
        {
            float rnd = Random.Range(-50.0f, 50.0f);

            Vector3 pos = _spawnPoint.position;
            _spawnPoint.position = new Vector3(pos.x + rnd, pos.y, pos.z + rnd);

            int index = GameManager.Instance.UserPlayerPrefab.GetCharacterIndex();

            bool isLocal = (PhotonNetwork.connected == false);
            CharacterManager.Instance.SpawnCharacter(index, _spawnPoint.position, _spawnPoint.rotation, isLocal);
        }

        void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom");

            // 로비로 이동
            SceneLoader.Instance.LoadScene("Lobby");
        }

        /// <summary>
        /// 다른 플레이어가 룸에 들어올 경우 호출됨
        /// </summary>
        /// <param name="player"></param>
        public void OnPhotonPlayerConnected(PhotonPlayer player)
        {
            //Debug.Log("OnPhotonPlayerConnected : " + player.NickName);

            //GameObject playerCharGo = player.TagObject as GameObject;
        }

        void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
        {
            Debug.Log("OnPhotonPlayerDisconnected");
        }
    }
}
