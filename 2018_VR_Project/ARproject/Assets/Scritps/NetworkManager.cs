using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : PunBehaviour, IPunTurnManagerCallbacks
{
    public bool isSingle = false; // 싱글플레이 테스트

    static public NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();

                if (FindObjectsOfType<NetworkManager>().Length > 1)
                {
                    return _instance;
                }
            }
            return _instance;
        }
    }

    private static NetworkManager _instance;

    private PunTurnManager turnManager;

    private Vector3 otherPlayerStackPos = Vector3.zero;

    // 현재 턴에서 플레이를 하고 있는 플레이어 아이디
    private int curTurnPlayerID;
    public int CurTurnPlayerID { get { return curTurnPlayerID; } }

    public TheStack theStack;

	void Start ()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        turnManager = gameObject.AddComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        //turnManager.TurnDuration = 5f; // 턴진행시간

        // 임의로 방장을 첫번째 플레이어로 지정
        if (PhotonNetwork.masterClient != null)
        {
            curTurnPlayerID = PhotonNetwork.masterClient.ID;

            theStack.UpdateText(curTurnPlayerID);

            if (turnManager.Turn == 0)
            {
                StartTurn();
            }
        }
    }

    /// <summary>
    /// 턴 시작
    /// </summary>
    public void StartTurn()
    {
        Debug.Log("StartTurn : " + PhotonNetwork.isMasterClient);

        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();
        }
    }

    public void SendTurn(Vector3 pos)
    {
        turnManager.SendMove(pos, true);
    }

    public void SendGameEnd()
    {
        // 다른플레이어들에게 게임종료 메세지를 보냄
        photonView.RPC("GameEnd", PhotonTargets.Others);
    }

    public void SendGameRestart(string sceneName)
    {
        photonView.RPC("GameRestart", PhotonTargets.Others, sceneName);
    }

    [PunRPC]
    public void GameEnd()
    {
        Debug.Log("GameEnd");

        theStack.EndGame();
    }

    [PunRPC]
    public void GameRestart(string sceneName)
    {
        Debug.Log("GameRestart");

        theStack.RestartGame();
    }

    public void OnEndTurn()
    {
        StartTurn();
    }

    public Vector3 GetOtherPlayerStackPos()
    {
        return otherPlayerStackPos;
    }

    /// <summary>
    /// 내가 방에 들어갈 때 호출
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom - test");   
    }

    /// <summary>
    /// 다른 플레이어가 접속 시 호출 (Game 씬만 별도로 실행 시 호출)
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Other player arrived");

        // 일단 테스트용으로 작성
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            // 임의로 방장을 첫번째 플레이어로 지정
            curTurnPlayerID = PhotonNetwork.masterClient.ID;

            theStack.UpdateText(curTurnPlayerID);

            if (turnManager.Turn == 0)
            {
                StartTurn();
            }
        }
    }

    #region TurnManager Callbacks

    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins : " + turn);
    }

    /// <summary>
    /// 모든 플레이어들이 한번씩 플레이를 한 경우 호출됨
    /// </summary>
    /// <param name="turn"></param>
    public void OnTurnCompleted(int turn)
    {
        Debug.Log("OnTurnCompleted : " + turn);

        OnEndTurn();
    }

    public void OnPlayerMove(PhotonPlayer player, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + player + " turn: " + turn + " action: " + move);
    }

    public void OnPlayerFinished(PhotonPlayer player, int turn, object move)
    {
        PhotonPlayer nextPlayer = player.GetNext();

        Debug.Log("OnTurnFinished: " + player + " turn: " + turn + " action: " + move + "next player: " + nextPlayer);

        curTurnPlayerID = nextPlayer.ID;
        theStack.UpdateText(curTurnPlayerID);

        otherPlayerStackPos = (Vector3)move;

        if (player.IsLocal == false)
        {
            theStack.UpdateTile();
        }
    }

    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("OnTurnTimeEnds : " + turn);
    }

    #endregion
}
