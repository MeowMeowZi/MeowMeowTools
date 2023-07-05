using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class TestATable
{
    public int Id;
    public string Test;
    public string Test1;
    public string Test2;
    public string Name;
    public int Age;
    public string AAAA;
}

public class TestACfg : ScriptableObject
{
    public List<TestATable> list;
    public Dictionary<int, TestATable> dict;
    
    public void Init()
    {
        dict = new Dictionary<int, TestATable>();
        foreach (var item in list)
        {
            dict.Add(item.Id, item);
        }
    }
}
