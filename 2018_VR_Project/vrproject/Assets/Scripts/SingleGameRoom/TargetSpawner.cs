using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    /// <summary>
    /// 타겟들을 생성하는 클랙스
    /// </summary>
    public class TargetSpawner : MonoBehaviour
    {
        public List<MovePath> pathList = new List<MovePath>();
        public List<GameObject> targetPrefabList = new List<GameObject>();

        // key : pathIndex, value : spawn object list
        Dictionary<int, List<GameObject>> _spawnList = new Dictionary<int, List<GameObject>>();

        void Start()
        {
            CreateSpawn();
        }

        void CreateSpawn()
        {
            for (int i = 0; i < pathList.Count; ++i)
            {
                GameObject target = SpawnTarget(i, pathList[i]);
                List<GameObject> newTargetList = new List<GameObject>();
                newTargetList.Add(target);

                _spawnList.Add(i, newTargetList);
            }
        }

        void OnEnable()
        {
            JellyTarget.OnDestroyTargetEvent += OnDestroyTarget;
        }

        void OnDisable()
        {
            JellyTarget.OnDestroyTargetEvent -= OnDestroyTarget;
        }

        void Update()
        {
            // 타겟 제거 테스트
            if (Input.GetMouseButtonDown(0))
            {
                int index = Random.Range(0, _spawnList.Count);
                List<GameObject> targetList = _spawnList[index];

                int pathIndex = targetList[0].GetComponent<JellyTarget>().pathIndex;
                DestroyImmediate(targetList[0]);

                OnDestroyTarget(pathIndex);
            }
        }

        /// <summary>
        /// 타겟생성
        /// </summary>
        /// <param name="pathIndex"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        GameObject SpawnTarget(int pathIndex, MovePath movePath)
        {
            Debug.Assert(targetPrefabList.Count > 0);

            // 프리팹 랜덤 선택
            int index = Random.Range(0, targetPrefabList.Count);
            GameObject prefab = targetPrefabList[index];

            // 타겟생성
            GameObject target = Instantiate(prefab, movePath.spawnPoint.transform.position, Quaternion.identity) as GameObject;
            target.SetActive(true);

            JellyMove jellyMove = target.GetComponent<JellyMove>();
            if (jellyMove != null)
            {
                movePath.dummy.transform.position = movePath.spawnPoint.transform.position;
                jellyMove.dummy = movePath.dummy;
                jellyMove.path = movePath.path;
            }

            JellyTarget jellyTarget = target.GetComponent<JellyTarget>();
            if (jellyTarget != null)
            {
                jellyTarget.pathIndex = pathIndex;
            }

            return target;
        }        

        /// <summary>
        /// 타겟 삭제 시 호출되는 이벤트 함수
        /// </summary>
        void OnDestroyTarget(int pathIndex)
        {
            if (_spawnList.ContainsKey(pathIndex) == true)
            {
                List<GameObject> targetList = _spawnList[pathIndex];
                Debug.Assert(targetList.Count > 0);
                Debug.Assert(targetList[0] == null);
                targetList.Clear();

                LeanTween.delayedCall(2.0f, ()=> 
                {
                    // 새로운 타겟 생성
                    GameObject newTarget = SpawnTarget(pathIndex, pathList[pathIndex]);
                    targetList.Add(newTarget);
                });
            }
        }        
    }    

    [System.Serializable]
    public class MovePath
    {
        public LeanTweenPath path;
        public Transform spawnPoint;
        public GameObject dummy;
    }
}
