using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SocialVR
{
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        const string LOBBY = "Lobby";
        const string SINGLEGAMEROOM = "SingleGameRoom";
        const string MEETINGROOM = "MeetingRoom";

        public string prevSceneName = string.Empty; // 이전 씬의 이름
        public string curSceneName = string.Empty;  // 현재 씬의 이름

        public void LoadScene(string sceneName)
        {
            prevSceneName = SceneManager.GetActiveScene().name;
            curSceneName = sceneName;

            SceneManager.LoadScene(curSceneName);
        }

        public IEnumerator LoadSceneAsync(string sceneName)
        {
            prevSceneName = SceneManager.GetActiveScene().name;
            curSceneName = sceneName;

            if (PhotonNetwork.connected == true)
            {
                // 씬을 이동하는 동안 서버로부터 네트워크 메세지 수신 중단
                PhotonNetwork.isMessageQueueRunning = false;
            }

            AsyncOperation ao = SceneManager.LoadSceneAsync(curSceneName);
            yield return ao;
        }

        public bool IsLobby()
        {
            // Lobby 씬을 단독으로 실행할 경우
            if(curSceneName == "")
            {
                return (SceneManager.GetActiveScene().name == LOBBY);
            }

            return (curSceneName == LOBBY);
        }

        public bool IsSingleGameRoom()
        {
            if (curSceneName == "")
            {
                return (SceneManager.GetActiveScene().name == SINGLEGAMEROOM);
            }

            return (curSceneName == SINGLEGAMEROOM);
        }

        public bool IsMeetingRoom()
        {
            if (curSceneName == "")
            {
                return (SceneManager.GetActiveScene().name == MEETINGROOM);
            }

            return (curSceneName == MEETINGROOM);
        }
    }
}
