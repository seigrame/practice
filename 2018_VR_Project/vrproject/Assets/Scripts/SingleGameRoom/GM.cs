using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GM : Singleton<GM> {
   // public static GM current;

    private Vector3 StartPos_Vec_A;
    private Vector3 StartPos_Vec_B;
    private Vector3 StartPos_Vec_C;
    private Vector3 StartPos_Vec_D;
    private Vector3 StartPos_Vec_E;

    public GameObject StartPos_A;
    public GameObject StartPos_B;
    public GameObject StartPos_C;
    public GameObject StartPos_D;
    public GameObject StartPos_E;
    
	// Use this for initialization
	void Start () {
        StartPos_Vec_A = StartPos_A.transform.position;
        StartPos_Vec_B = StartPos_B.transform.position;
        StartPos_Vec_C = StartPos_C.transform.position;
        StartPos_Vec_D = StartPos_D.transform.position;
        StartPos_Vec_E = StartPos_E.transform.position;
	}

    public void Active_PoolObj_A()
    {
        GameObject jelly_Obj_A = SpawnManager.current.GetPooledObject_A();

        if (jelly_Obj_A == null) return;

        jelly_Obj_A.transform.position = StartPos_Vec_A;
        jelly_Obj_A.SetActive(true);
    }

    public  void Active_PoolObj_B()
    {
        GameObject jelly_Obj_B = SpawnManager.current.GetPooledObject_B();

        if (jelly_Obj_B == null) return;

        jelly_Obj_B.transform.position = StartPos_Vec_B;
        jelly_Obj_B.SetActive(true);
    }

    public void Active_PoolObj_C()
    {
        GameObject jelly_Obj_C = SpawnManager.current.GetPooledObject_C();

        if (jelly_Obj_C == null) return;

        jelly_Obj_C.transform.position = StartPos_Vec_C;
        jelly_Obj_C.SetActive(true);
    }


    public  void Active_PoolObj_D()
    {
        GameObject jelly_Obj_D = SpawnManager.current.GetPooledObject_D();

        if (jelly_Obj_D == null) return;

        jelly_Obj_D.transform.position = StartPos_Vec_D;
        jelly_Obj_D.SetActive(true);
    }

    void Active_PoolObj_E()
    {
        GameObject jelly_Obj_E = SpawnManager.current.GetPooledObject_E();

        if (jelly_Obj_E == null) return;

        jelly_Obj_E.transform.position = StartPos_Vec_E;
        jelly_Obj_E.SetActive(true);
    }
}
