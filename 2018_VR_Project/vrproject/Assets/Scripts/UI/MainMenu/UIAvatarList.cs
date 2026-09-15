using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class UIAvatarList : MonoBehaviour
    {
        public GameObject itemPrefab = null;

        public void Init()
        {
            // 캐릭터 리스트 생성
            List<GameObject> characterList = CharacterManager.Instance.characterList;

            for (int i = 0; i < characterList.Count; ++i)
            {
                GameObject characterGo = characterList[i];
                Character character = characterGo.GetComponent<Character>();

                GameObject go = (GameObject)Instantiate(itemPrefab);
                go.name = string.Format("{0}_{1}", go.name, i);
                go.transform.SetParent(transform, false);

                UIAvatarItem item = go.GetComponent<UIAvatarItem>();
                if (character.info.image != null)
                {
                    item.iconImage.sprite = character.info.image;
                }

                int index = i;
                go.GetComponent<Button>().onClick.AddListener(delegate { OnClickAvatarItem(index); });
            }

            itemPrefab.gameObject.SetActive(false);
        }

        void OnClickAvatarItem(int index)
        {
            Debug.Log("OnClickAvatarItem : " + index);

            // 일단 임시로 캐릭터 변경처리
            CharacterManager.Instance.ChangeCharacter(index);
        }
    }
}