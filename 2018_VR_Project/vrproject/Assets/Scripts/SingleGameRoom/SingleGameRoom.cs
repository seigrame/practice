using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class SingleGameRoom : MonoBehaviour
    {
        [SerializeField]
        GameObject _characterPrefab = null;

        [SerializeField]
        Transform _spawnPoint = null;

        public List<SoundSource> bgmList = new List<SoundSource>();

        void Awake()
        {
            UIMainMenu.Instance.Init();

            //Instantiate(_characterPrefab, _spawnPoint.position, Quaternion.identity);

            // 배경사운드 랜덤으로 재생
            int index = Random.Range(0, bgmList.Count);
            SoundSource bgm = bgmList[index]; 
            SoundManager.Instance.PlayBGM(bgm.name);
        }
    }
}
