using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace SocialVR
{
    public class UIMainControl : MonoBehaviour
    {
        public Canvas canvas = null;

        void Start()
        {
            // PC 에서 테스트 시 vrtk ui canvas 비활성화 처리
            if (SteamVR.instance == null)
            {
                canvas.GetComponent<VRTK_UICanvas>().enabled = false;
            }
        }

        void Update()
        {
            // F1 키를 누르면 메인메뉴가 보임. PC 테스트용
            if (Input.GetKeyUp(KeyCode.F1))
            {
                bool isVisible = UIMainMenu.Instance.gameObject.activeSelf;
                UIMainMenu.Instance.ShowUI(isVisible == false);
            }
        }
    }
}
