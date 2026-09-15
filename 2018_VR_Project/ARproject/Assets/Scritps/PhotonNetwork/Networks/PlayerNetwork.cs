using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : MonoBehaviour
{   //싱글톤
    public static PlayerNetwork Instance;
    public string PlayerName { get; private set; } //읽기 가능, 쓰기 불가능
    private PhotonView PhotonView;
    int playerInGame = 0;

    private void Awake()
    {
        Instance = this;

        PhotonView = GetComponent<PhotonView>();
        PlayerName = "Guest#" + Random.Range(1000, 9999);

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    //로딩이 끝났을때
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Game")
        {
            if (PhotonNetwork.isMasterClient)
            { MasterLoadedGame(); }
            else
            { NonMasterLoadedGame(); }
        }
    }
    //방장의 로딩
    void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
        PhotonView.RPC("RPC_LoadedGameOthers", PhotonTargets.Others);
    }
    //게스트의 로딩
    void NonMasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient);
    }

    [PunRPC]
    void RPC_LoadedGameOthers()
    {
        Debug.Log("RPC_LoadedGameOthers");

        PhotonNetwork.LoadLevel(2); //Load레벨은 게임씬의 번호
    }
    [PunRPC]
    void RPC_LoadedGameScene()
    {
        Debug.Log("RPC_LoadedGameScene");

        playerInGame++;             //로딩이 끝나면 카운터 1씩 증가
        if(playerInGame == PhotonNetwork.playerList.Length) //플레이어수와 카운터가 일치하면
        {
            print("All players are in game scene.");
        }
    }
}
