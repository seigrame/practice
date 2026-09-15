using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Text scoreText;

    private void Start()
    {
        // 저장된 점수를 표시
        scoreText.text = PlayerPrefs.GetInt("score").ToString();
    }

    public void ToLobby()
    {
        // 게임씬으로 이동
        SceneManager.LoadScene("Lobby");
    }
}
