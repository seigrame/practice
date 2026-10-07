using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ML_FlowerArea : MonoBehaviour
{
    //꽃이 있는 지역의 지름
    //에이전트와 꽃 사이의 거리를 관찰하여 반경을 조절. 즉, 훈련지역의 반경을 뜻함
    public const float AreaDiameter = 20f;

    //꽃이 있는 지역의 모든 꽃의 종류를 리스트로
    [SerializeField] private List<GameObject> flowerPlants;
    //꿀이 있는 꽃과 콜라이더를 매핑하는 딕셔너리
    private Dictionary<Collider, ML_Flower> nectarFlowerDict;
    //꽃의 리스트
    public List<ML_Flower> flowers { get; private set; }
    //꽃 재설정
    public void ResetFlower()
    {
        //각각의 꽃 위치를 랜덤하게 재설정
        foreach (GameObject flowerplant in flowerPlants)
        {
            float xRotation = Random.Range(-5f, 5f);
            float yRotation = Random.Range(-180f, 180f);
            float zRotation = Random.Range(-5f, 5f);

            flowerplant.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }
        //꽃의 상태를 초기화
        foreach (ML_Flower flower in flowers)
        {
            flower.ResetFlower();
        }
    }
    /// <summary>
    /// 꽃의 콜라이더를 통해 꽃의 꿀을 가져오는 메서드 <see cref ="ML_Flower"/>.GetFlowerFromNectar(Collider col)
    /// </summary>
    /// <param name="col">넥타르 콜라이더</param>
    /// <returns></returns>
    public ML_Flower GetFlowerFromNectar(Collider col)
    {
        return nectarFlowerDict[col];
    }
    private void Awake()
    {
        flowerPlants = new List<GameObject>();
        nectarFlowerDict = new Dictionary<Collider, ML_Flower>();
        flowers = new List<ML_Flower>();     
    }
    private void Start()
    {
        //자식 오브젝트 중 ML_Flower 스크립트가 붙어있는 오브젝트를 찾아서 리스트에 추가
        FindChildFlowers(transform);
    }
    //화단에서 꽃을 찾는 재귀함수
    private void FindChildFlowers(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++) //이 스크립트가 붙어있는 오브젝트의 자식 오브젝트를 순회
        {
            Transform child = parent.GetChild(i); //변수 child는 계속 순회하면서 자식 오브젝트를 가리킴

            if (child.CompareTag("flower_plant")) //화단일 경우
            {
                flowerPlants.Add(child.gameObject); // 리스트에 추가

                FindChildFlowers(child); //현재 오브젝트가 자식 오브젝트를 가졌을 경우 그 밑으로 순회
            }
            else //화단이 아닐 경우
            {
                ML_Flower flower = child.GetComponent<ML_Flower>();
                if (flower != null)//꽃을 찾은 경우(꽃의 수술, 넥타르가 있는 부분)
                {
                    flowers.Add(flower);//꽃 리스트에 추가
                    nectarFlowerDict.Add(flower.nectarCols, flower);//꽃의 넥타르 콜라이더와 꽃을 매핑
                    //note : 다른 꽃들의 자식 오브젝트에는 꽃이 없습니다.
                }
                else
                {
                    //꽃을 찾지 못한경우
                    FindChildFlowers(child);
                }
            }
        }
    }
}