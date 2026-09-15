using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TheStack : MonoBehaviour
{
    public Text scoreText;
    public Color32[] gameColors = new Color32[4];
    //public Material stackMat;
    public Material stackMat1;
    public Material stackMat2;
    public GameObject endPanel;
    public Text turnPlayerName;

    private const float BOUNDS_SIZE = 3.5f;
    private const float STACK_MOVING_SPEED = 5.0f;
    private const float ERROR_MARGIN = 0.1f;
    private const float STACK_BOUNDS_GAIN = 0.25f;
    private const float COMBO_START_GAIN = 3f;

    private GameObject[] theStack;
    private Vector2 stackBounds = new Vector2(BOUNDS_SIZE, BOUNDS_SIZE);

    private int stackIndex;
    private int scoreCount = 0;
    private int combo = 0;

    private float tileTransition = 0f;
    private float tileSpeed = 2.5f;
    private float secondaryPosition;

    private bool isMovingOnX = true;
    private bool gameOver = false;

    private Vector3 desiredPosition;
    private Vector3 lastTilePosition;

	void Start ()
    {
        NetworkManager.Instance.theStack = this;

        theStack = new GameObject[transform.childCount];
        for(int i = 0; i < transform.childCount; ++i)
        {
            theStack[i] = transform.GetChild(i).gameObject;
            ColorMesh(theStack[i].GetComponent<MeshFilter>().mesh);
        }

        stackIndex = transform.childCount - 1;
	}

    /// <summary>
    /// 잘리는 면을 생성
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="scale"></param>
    private void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();

        Material[] materials = new Material[2];
        materials[0] = stackMat2;
        materials[1] = stackMat1;
        go.GetComponent<MeshRenderer>().materials = materials;

        //go.GetComponent<MeshRenderer>().material = stackMat;
        //ColorMesh(go.GetComponent<MeshFilter>().mesh);

        // 사운드출력
        SoundManager.instance.PlaySound();
    }
		  
	void Update ()
    {
        if (gameOver)
            return;

        if (!PhotonNetwork.inRoom)
            return;

        if (NetworkManager.Instance.isSingle == true) // 싱글플레이 테스트코드
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (PlaceTile())
                {
                    SpawnTile();
                    scoreCount++;
                    scoreText.text = scoreCount.ToString();
                }
                else
                {
                    EndGame();
                }
            }
        }
        else
        {
            PhotonPlayer local = PhotonNetwork.player;

            if (local.ID == NetworkManager.Instance.CurTurnPlayerID) // 내 차례가 되었을 경우
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 pos = theStack[stackIndex].transform.position;
                    NetworkManager.Instance.SendTurn(pos);

                    if (PlaceTile())
                    {
                        SpawnTile();
                        scoreCount++;
                        scoreText.text = scoreCount.ToString();
                    }
                    else
                    {
                        // 다른 플레이어들에게 게임종료 메세지 보냄
                        NetworkManager.Instance.SendGameEnd();
                        EndGame();
                    }
                }
            }            
        }

        MoveTile();

        // Move the stack
        transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	}

    /// <summary>
    /// 타일위치 갱신. 다른 플레이어가 턴 완료시 호출
    /// </summary>
    public void UpdateTile()
    {
        Vector3 stackPos = NetworkManager.Instance.GetOtherPlayerStackPos();
        theStack[stackIndex].transform.position = stackPos;

        if (PlaceTile())
        {
            SpawnTile();
            scoreCount++;
            scoreText.text = scoreCount.ToString();
        }
    }

    public void UpdateText(int playerID)
    {
        if(turnPlayerName.gameObject.activeSelf == false)
        {
            turnPlayerName.gameObject.SetActive(true);
        }

        PhotonPlayer[] players = PhotonNetwork.playerList;
        for (int i = 0; i < players.Length; ++i)
        {
            if (players[i].ID == playerID)
            {
                turnPlayerName.text = players[i].NickName;
            }
        }
    }

    /// <summary>
    /// 타일 이동
    /// </summary>
    private void MoveTile()
    {
        if (gameOver)
            return;

        tileTransition += Time.deltaTime * tileSpeed;
        if(isMovingOnX)
            theStack[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
        else
            theStack[stackIndex].transform.localPosition = new Vector3(secondaryPosition, scoreCount, Mathf.Sin(tileTransition) * BOUNDS_SIZE);
    }

    void SpawnTile()
    {
        lastTilePosition = theStack[stackIndex].transform.localPosition;

        stackIndex--;
        if (stackIndex < 0)
            stackIndex = transform.childCount - 1;

        desiredPosition = (Vector3.down) * scoreCount;

        theStack[stackIndex].transform.localPosition = new Vector3(0, scoreCount, 0);
        theStack[stackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        ColorMesh(theStack[stackIndex].GetComponent<MeshFilter>().mesh);
    }

    bool PlaceTile()
    {
        Transform t = theStack[stackIndex].transform;

        if (isMovingOnX)
        {
            float deltaX = lastTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                // CUT THE TILE
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;

                float middle = lastTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                CreateRubble
                (
                    new Vector3((t.position.x > 0) 
                    ? t.position.x + (t.localScale.x / 2)
                    : t.position.x - (t.localScale.x / 2)
                    , t.position.y
                    , t.position.z),
                    new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z)
                );

                t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
            }
            else
            {
                // 콤보달성 시 크기가 다시 조금 늘어남
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.x += STACK_BOUNDS_GAIN;
                    if (stackBounds.x > BOUNDS_SIZE)
                        stackBounds.x = BOUNDS_SIZE;

                    float middle = lastTilePosition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
                }

                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }
        else
        {
            float deltaZ = lastTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                // CUT THE TILE
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;

                float middle = lastTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                CreateRubble
                (
                    new Vector3(t.position.x
                    , t.position.y
                    , (t.position.z > 0)
                    ? t.position.z + (t.localScale.z / 2)
                    : t.position.z - (t.localScale.z / 2)),
                    new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ))
                );

                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
            }
            else
            {
                if (combo > COMBO_START_GAIN)
                {
                    stackBounds.y += STACK_BOUNDS_GAIN;
                    if (stackBounds.y > BOUNDS_SIZE)
                        stackBounds.y = BOUNDS_SIZE;

                    float middle = lastTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);
                    t.localPosition = new Vector3(lastTilePosition.x, scoreCount, middle - (lastTilePosition.z / 2));
                }

                combo++;
                t.localPosition = new Vector3(lastTilePosition.x, scoreCount, lastTilePosition.z);
            }
        }

        secondaryPosition = (isMovingOnX) ? t.localPosition.x : t.localPosition.z;

        isMovingOnX = !isMovingOnX;

        return true;
    }

    private void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(scoreCount * 0.25f);

        for (int i = 0; i < vertices.Length; ++i)
        {
            colors[i] = Lerp4(gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);
        }

        mesh.colors32 = colors;
    }

    Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < 0.33f)
            return Color.Lerp(a, b, t / 0.33f);
        else if (t < 0.66f)
            return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
        else
            return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
    }

    public void EndGame()
    {
        // 점수저장
        if (PlayerPrefs.GetInt("score") < scoreCount)
        {
            PlayerPrefs.SetInt("score", scoreCount);
        }

        gameOver = true;
        endPanel.SetActive(true);
        theStack[stackIndex].AddComponent<Rigidbody>();        
    }

    /// <summary>
    /// 게임다시시작 (일단보류)
    /// </summary>
    public void RestartGame()
    {
        //endPanel.SetActive(false);
    }

    public void OnButtonClick(string sceneName)
    {
        //NetworkManager.Instance.SendGameRestart(sceneName);

        PhotonNetwork.LoadLevel(1); // 로비로 이동
        //SceneManager.LoadScene(sceneName);
    }
}
