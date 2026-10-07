using System.Diagnostics;
using UnityEngine;

public class ML_Flower : MonoBehaviour
{
    [Tooltip("꿀이 가득찬 상태의 색상")]
    public Color fullColor = new Color(1f, .0f, .3f); // 
    [Tooltip("꽃이 비어 있을 때의 색상")]
    public Color emptyColor = new Color(.5f, 0f, 1f); // 

    [HideInInspector]
    public Collider nectarCols;


    private Collider flowerCols;
    private Material flowerMat;
    //벌새와 접촉할 부분
    public Vector3 flowerUpVector
    {
        get { return nectarCols.transform.up; }
    }
    //꽃의 중심부
    public Vector3 flowerCenterPosition
    {
        get { return nectarCols.transform.position; }
    }
    //꿀의 양
    public float NectarAmount
    {
        get; private set;
    }

    //꿀이 남아있는지 확인
    public bool HasNectar
    {
        get { return NectarAmount > 0f; }
    }
    //꿀이 제거되는 양
    public float Feed(float amount)
    {
        //저장된 양 이상을 가져갈 수 없음
        float nectarTaken = Mathf.Clamp(amount,0 ,NectarAmount);
        NectarAmount -= amount;

        if (NectarAmount <= 0f)
        {
            //잔량을 0으로
            NectarAmount = 0f;
            //상호작용 불가로 바꿈
            flowerCols.gameObject.SetActive(false);
            nectarCols.gameObject.SetActive(false);
            flowerMat.SetColor("_BaseColor", emptyColor);
        }
        return nectarTaken;
    }

    //Reset이라는 이름의 내장함수가 이미 존재하고 있으므로 주의
    public void ResetFlower()
    {
        NectarAmount = 1f;
        //상호작용 활성화
        flowerCols.gameObject.SetActive(true);
        nectarCols.gameObject.SetActive(true);
        //색상 초기화
        flowerMat.SetColor("_BaseColor", fullColor);
    }

    private void Awake()
    {
        flowerMat = GetComponent<MeshRenderer>().material;
        flowerCols = transform.Find("FlowerCollider").GetComponent<Collider>();
        nectarCols = transform.Find("FlowerNectarCollider").GetComponent<Collider>();
       
        ResetFlower();
    }
}
