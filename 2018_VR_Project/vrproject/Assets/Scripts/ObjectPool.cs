using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class ObjectPool : MonoBehaviour
    {
        private List<GameObject> _pooledObjects = new List<GameObject>();
        private GameObject _pooledObj = null;

        private int _maxPoolSize = 0;

        private int _initialPoolSize = 0;

        public ObjectPool(GameObject obj, int maxPoolSize)
        {
            _maxPoolSize = maxPoolSize;
            _pooledObj = obj;
            _initialPoolSize = maxPoolSize;

            for (int i = 0; i < maxPoolSize; ++i)
            {
                GameObject nObj = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;                
                nObj.SetActive(false);
                nObj.name = _pooledObj.name;

                _pooledObjects.Add(nObj);
            }
        }

        public GameObject GetObject()
        {
            int count = _pooledObjects.Count;
            for (int i = 0; i < count; ++i)
            {
                if (_pooledObjects[i].activeSelf == false)
                {
                    _pooledObjects[i].SetActive(true);
                    return _pooledObjects[i];
                }
            }

            // 부족할 경우 예외 처리
            GameObject nObj = NewObject();

            return nObj;
        }

        public void ReturnObject(GameObject obj)
        {
            int count = _pooledObjects.Count;
            for (int i = 0; i < count; ++i)
            {
                if (_pooledObjects[i] == obj)
                    _pooledObjects[i].SetActive(false);
            }
        }

        GameObject NewObject()
        {
            GameObject nObj = GameObject.Instantiate(_pooledObj, Vector3.zero, Quaternion.identity) as GameObject;

            nObj.SetActive(true);
            nObj.name = _pooledObj.name;

            _pooledObjects.Add(nObj);

            ++_maxPoolSize;

            //Debug.Log(string.Format("ObjectPool - Name : {0}, Size : {1}", nObj.name, _maxPoolSize));

            return nObj;
        }
    }
}
