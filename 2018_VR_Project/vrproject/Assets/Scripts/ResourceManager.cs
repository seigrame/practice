using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace SocialVR
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        ObjectPoolManager _poolManager = new ObjectPoolManager();

        //=================================================
        // Load
        // 1. Load From Object pool
        // 2. Load From Local assetBundle
        // 3. Load From Web assetBundle
        //=================================================
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            T retValue = null;

            GameObject pool = _poolManager.GetObject(path);
            if (pool != null)
            {
                retValue = pool.GetComponent<T>();
            }

            // local file load
            if (retValue == null)
            {
                string[] split = path.Split('_');

                StringBuilder builder = new StringBuilder();
                builder.Append(split[0]);
                builder.Append("/");
                builder.Append(path);

                var go = Resources.Load<GameObject>(builder.ToString());
                if (go != null)
                {
                    _poolManager.CreatePool(go, 1);

                    pool = _poolManager.GetObject(path);

                    if (pool != null)
                    {
                        retValue = pool.GetComponent<T>();
                    }
                }
            }

            return retValue;
        }

        public void LoadAsync<T>(string path, System.Action<T> callback) where T : UnityEngine.Object
        {
            var pool = _poolManager.GetObject(path);

            if (pool != null)
            {
                T retValue = pool.GetComponent<T>();

                callback(retValue);

                return;
            }

            // AssetBundle
            var go = Resources.Load<GameObject>(path);
            if (go != null)
            {
                T retValue = go.GetComponent<T>();

                _poolManager.CreatePool(go, 1);

                callback(retValue);
            }
        }

        public void ReturnObject(GameObject obj)
        {
            _poolManager.Return(obj);
        }
    }
}
