using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTK_Test : MonoBehaviour
{
    public GameObject prefab = null;
    public Transform spawnPoint = null;

	void Start ()
    {
        Instantiate(prefab, spawnPoint.position, Quaternion.identity);
	}
	
}
