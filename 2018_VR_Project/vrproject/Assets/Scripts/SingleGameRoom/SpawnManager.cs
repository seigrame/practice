using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager current;

    public GameObject PoolObj_A;
    public GameObject PoolObj_B;
    public GameObject PoolObj_C;
    public GameObject PoolObj_D;
    public GameObject PoolObj_E;

    public GameObject Jelly_Enemy_Objs;

    public int PoolAmount_A = 1;
    public int PoolAmount_B = 5;
    public int PoolAmount_C = 5;
    public int PoolAmount_D = 5;
    public int PoolAmount_E = 5;

    public List<GameObject> PoolObjs_A;
    public List<GameObject> PoolObjs_B;
    public List<GameObject> PoolObjs_C;
    public List<GameObject> PoolObjs_D;
    public List<GameObject> PoolObjs_E;

    private void Awake()
    {
        current = this;
    }
    // Use this for initialization
    void Start () {
        Init_PoolObjs_A();
        Init_PoolObjs_B();
        Init_PoolObjs_C();
        Init_PoolObjs_D();
        Init_PoolObjs_E();
    }

    void Instantiate_PoolObjs(List<GameObject> PoolObjs_Param, GameObject PoolObj_Param, int PoolAmount_Param)
    {
        for(int i=0; i < PoolAmount_Param; i++)
        {
            GameObject Jelly_Obj = Instantiate(PoolObj_Param) as GameObject;  
            Jelly_Obj.transform.parent = Jelly_Enemy_Objs.transform;

            Jelly_Obj.SetActive(false);
            PoolObjs_Param.Add(Jelly_Obj);
        }
    }

    void Init_PoolObjs_A()
    {
        PoolObjs_A = new List<GameObject>();
        Instantiate_PoolObjs(PoolObjs_A, PoolObj_A, PoolAmount_A);
    }

    void Init_PoolObjs_B()
    {
        PoolObjs_B = new List<GameObject>();
        Instantiate_PoolObjs(PoolObjs_B, PoolObj_B, PoolAmount_B);
    }

    void Init_PoolObjs_C()
    {
        PoolObjs_C = new List<GameObject>();
        Instantiate_PoolObjs(PoolObjs_C, PoolObj_C, PoolAmount_C);
    }

    void Init_PoolObjs_D()
    {
        PoolObjs_D = new List<GameObject>();
        Instantiate_PoolObjs(PoolObjs_D, PoolObj_D, PoolAmount_D);
    }

    void Init_PoolObjs_E()
    {
        PoolObjs_E = new List<GameObject>();
        Instantiate_PoolObjs(PoolObjs_E, PoolObj_E, PoolAmount_E);
    }

    public GameObject GetPooledObject_A()
    {
        for(int i=0; i < PoolObjs_A.Count; i++)
        {
            if (!PoolObjs_A[i].activeInHierarchy)
            {
                return PoolObjs_A[i];
            }
        }
        return null;
    }

    public GameObject GetPooledObject_B()
    {
        for(int i=0; i < PoolObjs_B.Count; i++)
        {
            if(!PoolObjs_B[i].activeInHierarchy)
            {
                return PoolObjs_B[i];
            }
        }
        return null;
    }

    public GameObject GetPooledObject_C()
    {
        for(int i=0; i < PoolObjs_C.Count; i++)
        {
            if(!PoolObjs_C[i].activeInHierarchy)
            {
                return PoolObjs_C[i];
            }
        }
        return null;
    }

    public GameObject GetPooledObject_D()
    {
        for(int i=0; i < PoolObjs_D.Count; i++)
        {
            if(!PoolObjs_D[i].activeInHierarchy)
            {
                return PoolObjs_D[i];
            }
        }
        return null;
    }

    public GameObject GetPooledObject_E()
    {
        for(int i=0; i < PoolObjs_E.Count; i++)
        {
            if(!PoolObjs_E[i].activeInHierarchy)
            {
                return PoolObjs_E[i];
            }
        }
        return null;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
