using System;
using System.Collections.Generic;

public enum RewardStatType {
    None=0,
    MaxHp,
    MoveSpeed,
    Attack,
    MagnetRange,
    Armor,
    CriticalPercent
}
public enum StatClacType
{
    None = 0,
    Flat,
    Percent
}
public enum ItemType 
{
    None =0,
    DropItem,
    Coin,
    HpPotion,
    MagnetPotion,
    BerserkPotion
}
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
    public string SilhouetteIconPath;
    public string DropTableId;
    public int MonsterExp;
}
[System.Serializable]
public class ItemData : GameDataBase
{
    public string Name;
    public string Description;
    public ItemType EItemType;
    public int MaxStackCount; 
    public int SellingPrice;  
    public string PrefabPath;
    public string IconPath;
    public string Grade;
    public bool IsUseable;
}
[System.Serializable]
public class BulletData : GameDataBase
{
    public string Name;
    public string Description;
    public float MoveSpeed;
    public float DestroyTime;
    public int Damage;
    public int FireSpeed;
    public string PrefabPath;
    public string IconPath;
    public string UpgradeIngredientTableId;
    public string NextBulletId;


}
[System.Serializable]
public class DropTableData : GameDataBase 
{
    public int CoinMinAMount; public int CoinMaxAMount;  
    public string DropItemId1; public int Item1DropPercent;
    public string DropItemId2; public int Item2DropPercent;
    public string DropItemId3; public int Item3DropPercent;
}
[System.Serializable]
public class LevelUpRewardData : GameDataBase 
{
    public string Name;
    public string Description;
    public RewardStatType StatType;
    public StatClacType ClacType;
    public float Value;
    public int MaxLevel;
    public string IconPath;
}
[System.Serializable]
public class ShopItemData : GameDataBase
{
    public string Name;
    public string Description;
    public ItemType EItemType;
    public int MaxStackCount;
    public int SellingPrice;
    public string IconPath;
}
[System.Serializable]
public class StageMonsterTableData : GameDataBase
{
    public string MonsterId1;
    public string MonsterId2;
    public string MonsterId3;
}
[System.Serializable]
public class BombData : GameDataBase
{
    public string Name;
    public string Description;
    public int BombDamage;
    public float ExplosionRadius;
    public float MoveSpeed;
    public float CoolTime;
    public string IconPath;
    public string PrefabPath;
    public string UpgradeitemId;
}
[System.Serializable]
public class BulletUpgradeTableData : GameDataBase
{
    public int CoinMount;
    public string ItemId1; public int Item1Amount;
    public string ItemId2; public int Item2Amount;
    public string ItemId3; public int Item3Amount;
}