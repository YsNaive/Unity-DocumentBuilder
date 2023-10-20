using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestBased
{
    public TestBased()
    {
        Debug.Log("CREATE");
    }
    ~TestBased()
    {
        Debug.Log("DELETE");
    }
}

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        //(int x, int y) p = (10, 10);
        
        //Debug.Log(JsonUtility.ToJson(p));
        //Debug.Log(p.GetType());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
