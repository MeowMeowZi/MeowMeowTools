using System.Collections;
using System.Collections.Generic;
using MeowMeowTools.ExcelTools;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        Debug.Log($"---{ExcelManager.Instance.CfgTestA[0].name}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
