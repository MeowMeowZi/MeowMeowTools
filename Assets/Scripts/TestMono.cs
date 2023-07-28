using System.Collections;
using System.Collections.Generic;
using MeowMeowTools.ExcelTools;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    // Start is called before the first frame update
    void Start(){
        CfgTestA cfgTestA = ExcelManager.Instance.TableTestA[0];
        Debug.Log($"---{cfgTestA.id}");
        Debug.Log($"---{cfgTestA.name}");
        Debug.Log($"---{cfgTestA.age}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
