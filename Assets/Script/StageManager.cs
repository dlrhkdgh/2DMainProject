using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Inst { get; set; }
    [SerializeField] BulletSpawner _bulletSpawner;
    [SerializeField] MonsterSpawner _monsterSpawmer1;
    [SerializeField] MonsterSpawner _monsterSpawmer2;
    [SerializeField] DropItemSpawner _dropItemSpawner;
    public int StageGold { get; set; } = 0;
    public Dictionary<string, int> _stageInventoryDic = new Dictionary<string, int>();
    public Dictionary<string, int> _currentLevelUpRewardDic = new Dictionary<string, int>();
    public Dictionary<string, int> _potionInventoryDic = new Dictionary<string, int>();
    public List<string> _finalRewardList = new List<string>();
    public int PlayerExp { get; set; } = 0;
    public int maxExp = 300;
    public int PlayerLevel { get; set; } = 0;
    public Action<int, int, int> OnExpChanged;
    public Action OnPlayerStatChanged;
    //public Action<int> OnMaxHpChanged;
    private bool isStageOnGoing = false;
    private void Awake()
    {
        Inst = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitPotionInventory();
        //DataManager.Inst.LoadFullData();
        // StageStart();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void StartStage(int stageNum)
    {
        if (isStageOnGoing) return;
      
        _monsterSpawmer1.InitMonsterSpawner(Player.Inst.transform);
        _monsterSpawmer2.InitMonsterSpawner(Player.Inst.transform);
        _dropItemSpawner.InitDropItemSpawner();
        _bulletSpawner.InitBulletSpawner();
        ResetLevel();
        InitCurrentLevelUpRewardDic();
        Player.Inst.StartShooting();

        isStageOnGoing=true;
    }
    public void DropItemFromMonster(Vector3 diePosition, string monsterDropTableId)
    {

        _dropItemSpawner.DropItemInField(diePosition, monsterDropTableId);

    }
    public void StartFireBullet(Vector3 spawnPosition, Vector2 direction)
    {
        _bulletSpawner.FireBullet(spawnPosition, direction);
    }
    public void FinishStage()
    {
        if (isStageOnGoing == false) return;

        _monsterSpawmer1.ClearAndReleaseSpawner();
        _monsterSpawmer2.ClearAndReleaseSpawner();
        _dropItemSpawner.ClearAndReleaseSpawner();
        _bulletSpawner.ClearAndReleaseSpawner();
        Player.Inst.StopShooting();

        AddToRealInventory(1f);
        ResetStageInfo();

        isStageOnGoing = false;
    }
    public bool AddStageGold(int getGold)
    {

        int newGold = StageGold + getGold;
        if (newGold < 0)
        {
            return false;
        }
        else
        {
            StageGold = newGold;
            return true;

        }
    }
    public void AddStageInventory(string itemId, int itemCount)
    {

        if (string.IsNullOrEmpty(itemId) || itemCount == 0) return;

        if (_stageInventoryDic.ContainsKey(itemId))
        {
            _stageInventoryDic[itemId] = _stageInventoryDic[itemId] + itemCount;
            //DebugPrintInventory();
        }
        else
        {
            _stageInventoryDic.Add(itemId, itemCount);
            //DebugPrintInventory();
        }
    }
    public void ResetStageInfo() {

        StageGold = 0;
        _stageInventoryDic.Clear();
        _currentLevelUpRewardDic.Clear();
        Player.Inst.ResetAddedPlayerStageStat();
    }
    public void AddToRealInventory(float scalef) {

        GameManager.Inst.AddGold(StageGold);
        foreach (KeyValuePair<string, int> item in _stageInventoryDic)
        {
        GameManager.Inst.AddInventory(item.Key, item.Value);
        }
    }
    public void PlayerGetExp(int expAmount) {
        PlayerExp = PlayerExp+ expAmount;
        while (PlayerExp >= maxExp) 
        {
            PlayerExp = PlayerExp - maxExp;
            PlayerLevelUp();
        }
        OnExpChanged?.Invoke(PlayerExp, maxExp, PlayerLevel);
    }
    public void PlayerLevelUp() {
        PlayerLevel++;
        GetLevelUpReward();

       //maxExp = Mathf.RoundToInt(maxExp * 1.2f);
    }
    public void ResetLevel() {
        PlayerLevel = 0;
        PlayerExp = 0;    
    }
    public void GetLevelUpReward() 
    {
        GameManager.Inst.PauseGame();
        _finalRewardList.Clear();
        _finalRewardList = GetRandomLevelUpReward(3);
        UIManager.Inst.OpenLevelUpRewardUI();
    }
    public void FinishLevelUpReward() 
    {
        UIManager.Inst.CloseLevelUpRewardUI();
        GameManager.Inst.ResumeGame();
    }
    public void InitCurrentLevelUpRewardDic() {
        foreach (KeyValuePair<string, LevelUpRewardData> reward in DataManager.Inst.LevelUpRewardDataList)
        {
            _currentLevelUpRewardDic.Add(reward.Key,0);
        }    
    }
    public List<string> GetRandomLevelUpReward(int count) {
       
        List<string> finalReward = new List<string>();

        foreach (KeyValuePair<string, LevelUpRewardData> data in DataManager.Inst.LevelUpRewardDataList)
        {
            if (!(CheckRewardIsMaxLevel(data.Key))) {
            finalReward.Add(data.Key);            
            }        
        }
        if (finalReward.Count <= count)
        {
            return finalReward;
        }
        for (int i = 0; i < finalReward.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, finalReward.Count);
            string temp = finalReward[i];
            finalReward[i] = finalReward[randomIndex];
            finalReward[randomIndex] = temp;
        }
        
        return finalReward.GetRange(0, count);
    }
    public bool CheckRewardIsMaxLevel(string id) {
        LevelUpRewardData data = DataManager.Inst.GetLevelUpRewardData(id);

        if (_currentLevelUpRewardDic[id] >= data.MaxLevel) return true;
        else return false;
    }
    public void PlayerGetLevelUpReward(string id) {
        Debug.Log($"레벨업 할 스텟{id}");
        _currentLevelUpRewardDic[id]++;
        PlayerStatLevelUp(id);
        OnPlayerStatChanged?.Invoke();
    }
    public void PlayerStatLevelUp(string id) {
        LevelUpRewardData data= DataManager.Inst.GetLevelUpRewardData(id);
        RewardStatType type = data.StatType;
        Debug.Log($"레벨업 할 스텟{type}");
        switch (type) {
            case RewardStatType.MaxHp: PlayerMaxHpLevelUp(data); Debug.Log($"변경후 스텟{Player.Inst._stageAddMaxHpPercent}"); break;
            case RewardStatType.MoveSpeed: Player.Inst._stageAddMoveSpeed += data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddMoveSpeed}"); break;
            case RewardStatType.Attack: Player.Inst._stageAddAttack += (int)data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddAttack}"); break;
            case RewardStatType.MagnetRange: Player.Inst._stageAddMagnetRange += data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddMagnetRange}"); break;
            case RewardStatType.Armor: Player.Inst._stageAddArmor += (int)data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddArmor}"); break;
            case RewardStatType.CriticalPercent: Player.Inst._stageAddCriticalPercent += (int)data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddCriticalPercent}"); break;
            default:break;
        }
    }
    public void PlayerMaxHpLevelUp(LevelUpRewardData data) {
        Player.Inst._stageAddMaxHpPercent += data.Value;
        Player.Inst.GetLevelUpHp(data.Value);
    }
    public void InitPotionInventory() {
        _potionInventoryDic.Add("item_potion_02", 5);
        _potionInventoryDic.Add("item_potion_03", 3);
        _potionInventoryDic.Add("item_potion_01", 8);

    }
}
