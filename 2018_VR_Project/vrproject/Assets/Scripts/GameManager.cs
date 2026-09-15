using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialVR
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        UserPlayerPrefab _userPlayerPrefab;
        public UserPlayerPrefab UserPlayerPrefab { get { return _userPlayerPrefab; } }

        protected override void Awake()
        {
            base.Awake();

            if (_userPlayerPrefab == null)
            {
                _userPlayerPrefab = new UserPlayerPrefab();
            }
        }        
    }
}
