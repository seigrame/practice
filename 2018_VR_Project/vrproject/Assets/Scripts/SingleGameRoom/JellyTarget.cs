using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class JellyTarget : MonoBehaviour
    {
        public delegate void DestroyEventHander(int pathIndex);
        public static event DestroyEventHander OnDestroyTargetEvent;

        private bool targetEnabled = true;
        public GameObject jellyParticle = null;

        public int pathIndex = 0;

        void ApplyDamage()
        {
            // 파티클 출력
            GameObject jellyInstance = Instantiate(jellyParticle, transform.position, Quaternion.identity);
            jellyParticle.transform.localScale = new Vector3(10f, 10f, 10f);

            // 파티클 제거
            Destroy(jellyInstance, 3f);

            // 오브젝트 제거
            Destroy(gameObject);

            // 이벤트함수 호출
            OnDestroyTargetEvent(pathIndex);
        }

        IEnumerator DectiveCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            gameObject.SetActive(false);
        }

        void Jelly_Active()
        {
            //    GM.current.Active_PoolObj_A();
            GM.Instance.Active_PoolObj_A();
        }
    }
}
