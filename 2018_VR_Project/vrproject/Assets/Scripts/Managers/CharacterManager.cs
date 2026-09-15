using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        public List<GameObject> characterList = new List<GameObject>();

        int _curCharIndex = 0;              // 현재 선택된 캐릭터의 인덱스
        GameObject _curCharacter = null;

        bool _isThirdPerson = false;

        public void Init(bool isThirdPerson = false)
        {
            _isThirdPerson = isThirdPerson;

            characterList = CharacterDB.Load();
            Debug.Assert(characterList != null);
        }

        public void ChangeCharacter(int index)
        {
            if (_curCharIndex == index)
                return;

            // 캐릭터에 붙어 있던 카메라를 분리
            //FirstPersonCamera cameraScript = _curCharacter.GetComponent<FirstPersonCamera>();
            //cameraScript.cam.transform.parent = null;

            // 캐릭터의 위치값을 가져옴
            Vector3 prevPos = _curCharacter.transform.position;
            Quaternion rot = _curCharacter.transform.rotation;

            // 캐릭터 삭제
            Destroy(_curCharacter);

            // 캐릭터 생성
            _curCharIndex = index;
            SpawnCharacter(_curCharIndex, prevPos, rot, true);

            GameManager.Instance.UserPlayerPrefab.SetCharacterIndex(_curCharIndex);
        }

        public void SpawnCharacter(int index, Vector3 pos, Quaternion rot, bool isLocal)
        {
            _curCharIndex = index;
            GameObject go = characterList[_curCharIndex];

            if (isLocal == true)
            {
                _curCharacter = Instantiate(go, pos, rot);

                CharacterNetwork characterNetwork = _curCharacter.GetComponent<CharacterNetwork>();
                characterNetwork.isThirdPersonCharacter = _isThirdPerson;
            }
            else
            {
                _curCharacter = PhotonNetwork.Instantiate(go.name, pos, Quaternion.identity, 0);
            }
        }        
    }
}
