using System;
using System.Collections.Generic;
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
    public int PlayerExp { get; set; } = 0;
    public int maxExp = 300;
    public int PlayerLevel { get; set; } = 0;
    public Action<int, int, int> OnExpChanged;
    private bool isStageOnGoing = false;
    private void Awake()
    {
        Inst = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        ResetStageInventory();

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
    public void ResetStageInventory() {

        StageGold = 0;
        _stageInventoryDic.Clear();
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
        UIManager.Inst.OpenLevelUpRewardUI();
    }
    public void FinishLevelUpReward() 
    {
        UIManager.Inst.CloseLevelUpRewardUI();
        GameManager.Inst.ResumeGame();
    }
}
