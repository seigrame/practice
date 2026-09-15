using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class ObjectPoolManager
    {
        Dictionary<string, ObjectPool> _dicPool = new Dictionary<string, ObjectPool>();

        public bool CreatePool(GameObject objToPool, int maxPoolSize)
        {
            if (_dicPool.ContainsKey(objToPool.name))
            {
                return false;
            }
            else
            {
                ObjectPool nPool = new ObjectPool(objToPool, maxPoolSize);
                _dicPool.Add(objToPool.name, nPool);

                return true;
            }
        }

        public GameObject GetObject(string path)
        {
            if (_dicPool.ContainsKey(path))
                return _dicPool[path].GetObject();

            return null;
        }        

        public void Return(GameObject obj)
        {
            _dicPool[obj.name].ReturnObject(obj);
        }        
    }
}
