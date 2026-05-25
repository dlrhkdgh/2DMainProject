using System;
using System.Collections.Generic;

[ System.Serializable]
public class GameDataBase
{
    public string Id;
}
[System.Serializable]
public class PlayerData : GameDataBase 
{
    public string Name;
    public int MaxHp;
    public int AttackDamage;
    public float AttackSpeed;
    public float MoveSpeed;
    public string PrefabPath;
    public string IconPath;
}
[System.Serializable]
public class MonsterData : GameDataBase
{
    public string Name;
    public string Description;
    public int MaxHp;
    public int AttackDamage;
    public float MoveSpeed;
    public string PrefabPath;
    public string IconPath;
    public string DropTableId;
    public int MonsterExp;
}
[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string Description;
    public string ItemType;
    public int MaxStackCount; 
    public int SellingPrice;  
    public string PrefabPath;
    public string IconPath;
    public string Grade;
}
[System.Serializable]
public class BulletData : GameDataBase
{
    public string Name;
    public string Description;
    public float MoveSpeed;
    public float DestroyTime;
    public int Damage;
    public int BulletLevel;
    public string PrefabPath;
    public string IconPath;
    
}
[System.Serializable]
public class DropTableData : GameDataBase 
{
    public int CoinMinAMount; public int CoinMaxAMount;  
    public string DropItemId1; public int Item1DropPercent;
    public string DropItemId2; public int Item2DropPercent;
    public string DropItemId3; public int Item3DropPercent;
}