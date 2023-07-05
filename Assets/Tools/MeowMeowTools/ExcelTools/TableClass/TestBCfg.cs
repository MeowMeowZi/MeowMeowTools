using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class TestBTable
{
    public int Id;
    public string name;
    public string prefab;
    public int wandRank;
    public float imageSize;
    public float colliderSize;
    public float speed;
    public int manaVolume;
    public float manaRecoverySpeed;
    public int launchMode;
    public float launchCd;
    public int ammoID;
    public int movePathID;
    public float lanuchAngle;
    public float ammoMoveSpeed;
    public float ammoDamage;
    public float ammoSize;
    public float ammoLifeTime;
    public int ammoPassThrough;
    public float ammoRepel;
    public int ammoAmount;
    public int languageId;
}

public class TestBCfg : ScriptableObject
{
    public List<TestBTable> list;
    public Dictionary<int, TestBTable> dict;
    
    public void Init()
    {
        dict = new Dictionary<int, TestBTable>();
        foreach (var item in list)
        {
            dict.Add(item.Id, item);
        }
    }
}
