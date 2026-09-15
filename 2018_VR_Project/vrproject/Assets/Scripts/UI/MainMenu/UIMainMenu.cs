using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SocialVR
{
    public class UIMainMenu : Singleton<UIMainMenu>
    {
        private enum MenuType
        {
            Home,
            Room,
            Avatar,
            Emoticon,
            Setting
        }

        public Button homeButton = null;
        public Button roomButton = null;
        public Button avatarButton = null;
        public Button emoticonButton = null;
        public Button settingButton = null;

        public UIRoomMenu roomMenu = null;
        public UIAvatarMenu avatarMenu = null;
        public UICreateRoom createRoomListMenu = null;
        public GameObject emoticonPanel = null;
        public GameObject settingPanel = null;

        public Canvas canvas = null;

        protected override void Awake()
        {
            base.Awake();

            gameObject.SetActive(false);
        }

        public void Init()
        {
            if (SceneLoader.Instance.IsLobby() == true)
            {
                if (avatarMenu != null)
                {
                    avatarMenu.Init();
                }

                homeButton.interactable = false;
                roomButton.interactable = true;
                avatarButton.interactable = true;
                emoticonButton.interactable = false;
            }
            else if (SceneLoader.Instance.IsSingleGameRoom() == true)
            {
                roomButton.interactable = false;
                avatarButton.interactable = false;
                emoticonButton.interactable = false;
            }
            else if (SceneLoader.Instance.IsMeetingRoom() == true)
            {
                roomButton.interactable = false;
                avatarButton.interactable = false;
                emoticonButton.interactable = true;
            }
        }

        void OnEnable()
        {
            homeButton.onClick.AddListener(OnHoneButtonClicked);
            roomButton.onClick.AddListener(OnRoomButtonClicked);
            avatarButton.onClick.AddListener(OnAvatarButtonClicked);
            emoticonButton.onClick.AddListener(OnEmoticonButtonClicked);
            settingButton.onClick.AddListener(OnSettingButtonClicked);
        }

        void OnDisable()
        {
            homeButton.onClick.RemoveListener(OnHoneButtonClicked);
            roomButton.onClick.RemoveListener(OnRoomButtonClicked);
            avatarButton.onClick.RemoveListener(OnAvatarButtonClicked);
            emoticonButton.onClick.RemoveListener(OnEmoticonButtonClicked);
            settingButton.onClick.RemoveListener(OnSettingButtonClicked);
        }

        /// <summary>
        /// Home 메뉴 클릭 시 
        /// </summary>
        void OnHoneButtonClicked()
        {
            if (SceneLoader.Instance.IsLobby() == true)
                return;

            // 로비로 이동
            if (SceneLoader.Instance.IsSingleGameRoom() == true)
            {
                SceneLoader.Instance.LoadScene("Lobby");
            }
            else
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        /// <summary>
        /// Room 메뉴 클릭 시
        /// </summary>
        void OnRoomButtonClicked()
        {
            ShowPanel(MenuType.Room);
        }

        /// <summary>
        /// Avatar 메뉴 클릭 시 
        /// </summary>
        void OnAvatarButtonClicked()
        {
            ShowPanel(MenuType.Avatar);
        }

        /// <summary>
        /// Emoticon 메뉴 클릭 시
        /// </summary>
        void OnEmoticonButtonClicked()
        {
            ShowPanel(MenuType.Emoticon);
        }

        /// <summary>
        /// Setting 메뉴 클릭 시
        /// </summary>
        void OnSettingButtonClicked()
        {
            ShowPanel(MenuType.Setting);
        }

        void ShowPanel(MenuType menu)
        {
            if(roomMenu != null) roomMenu.gameObject.SetActive(menu == MenuType.Room);
            if(avatarMenu != null) avatarMenu.gameObject.SetActive(menu == MenuType.Avatar);
            if(emoticonPanel != null) emoticonPanel.SetActive(menu == MenuType.Emoticon);
            if(settingPanel != null) settingPanel.SetActive(menu == MenuType.Setting);
        }

        public void ShowUI(bool isShow)
        {
            gameObject.SetActive(isShow);

            if(roomMenu != null) roomMenu.gameObject.SetActive(false);
            if(avatarMenu != null) avatarMenu.gameObject.SetActive(false);
            if(emoticonPanel != null) emoticonPanel.SetActive(false);
            if(settingPanel != null) settingPanel.SetActive(false);
        }
    }
}
