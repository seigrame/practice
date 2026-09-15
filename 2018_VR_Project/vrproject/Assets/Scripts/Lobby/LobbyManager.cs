using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public enum RoomType
    {
        None,
        Meeting,
        SingleGame,     // 활쏘기
        MultiGame       // 멀티대전
    }

    public class LobbyManager : Photon.MonoBehaviour
    {
        public Transform spawnPoint = null;
        public SoundSource bgm = null;

        void Start()
        {
            CharacterManager.Instance.Init(true);
            UIMainMenu.Instance.Init();

            int index = GameManager.Instance.UserPlayerPrefab.GetCharacterIndex();

            // 추후에 PlayerPrefab 에 저장된 캐릭터의 인덱스를 가져와 생성해줌
            CharacterManager.Instance.SpawnCharacter(index, spawnPoint.position, spawnPoint.rotation, true);

            SoundManager.Instance.PlayBGM(bgm.name);
        }        
    }
}
