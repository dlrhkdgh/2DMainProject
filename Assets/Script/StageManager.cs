using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class StageManager : MonoBehaviour
{
    public static StageManager Inst { get; set; }
    [SerializeField] BulletSpawner _bulletSpawner;
    [SerializeField] BulletSpawner _bulletSpawnerForBoss;
    [SerializeField] MonsterSpawner _monsterSpawmer1;
    [SerializeField] MonsterSpawner _monsterSpawmer2;
    [SerializeField] DropItemSpawner _dropItemSpawner;
    [SerializeField] BombSpawner _bombSpawner;

    public int StageGold { get; set; } = 0;
    public Dictionary<string, int> _stageInventoryDic = new Dictionary<string, int>();
    public Dictionary<string, int> _currentLevelUpRewardDic = new Dictionary<string, int>();

    public Monster _targetBoss;
    public List<string> _finalRewardList = new List<string>();
    public int PlayerExp { get; set; } = 0;
    public int _maxExp = 300;
    private Queue<bool> _levelUpQueue = new Queue<bool>();
    private bool _isLevelUpUI = false;
    public int PlayerLevel { get; set; } = 0;
    public Action<int, int, int> OnExpChanged;
    public Action OnPlayerStatChanged;
    public Action OnUseableItemChanged;
    public Action OnSkillUse;
    //public Action<int> OnMaxHpChanged;
    private bool _isStageOnGoing = false;
    public bool _isClear;
    public float MaxStageTime { get; set; } = 180f;
    public float StageTimer { get; private set; } = 0f;

    public int _currentStageNum;

    private void Awake()
    {
        Inst = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (!_isStageOnGoing) return;

        StageTimer += Time.deltaTime;

        if (StageTimer >= MaxStageTime)
        {
            GameManager.Inst.FinishStage(true);
        }
    }
    public void StageStart(int stageNum)
    {
        if (_isStageOnGoing) return;

        _currentStageNum = stageNum;
        SetSpawners(stageNum);
        if (stageNum == 2) //boss
        {
            StartBossBattle().Forget();
            OnlySetSpawners();
            _bulletSpawnerForBoss.InitBulletSpawner(true);
        }
        else
        {
            StartSpawners();
        }
        ResetLevel();
        InitCurrentLevelUpRewardDic();
        Player.Inst.StartShooting();
        Player.Inst._isOnBattle = true;
        StageTimer = 0f;
        _isStageOnGoing = true;
    }
    public void DropItemFromMonster(Vector3 diePosition, string monsterDropTableId)
    {

        _dropItemSpawner.DropItemInField(diePosition, monsterDropTableId);

    }
    public void StartFireBullet(Vector3 spawnPosition, Vector2 direction)
    {
        _bulletSpawner.FireBullet(spawnPosition, direction);
    }
    public void StartFireBulletForBoss(Vector3 spawnPosition, Vector2 direction)
    {
        _bulletSpawnerForBoss.FireBullet(spawnPosition, direction);
    }
    public void ThrowBomb(Vector3 spawnPosition, Vector2 direction)
    {
        if (_bombSpawner.SpawnBomb(spawnPosition, direction))
        {
            OnSkillUse?.Invoke();
        }
    }
    public void SpawnEffect(Vector3 spawnPosition)
    {
        _bombSpawner.SpawnBombEffect(spawnPosition);
    }
    public void StageFinish()
    {
        if (_isStageOnGoing == false) return;

        ClearSpawners();
        if (_targetBoss != null) {
            Destroy(_targetBoss.gameObject);
        }
        Player.Inst.StopShooting();
        Player.Inst._isOnBattle = false;
        _currentStageNum = 0;
        if (_isClear)
        {
            AddToRealInventory(1f);
        }
        else
        {
            AddToRealInventory(0.5f);
        }
        ResetStageInfo();

        _isStageOnGoing = false;
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
    public void ResetStageInfo()
    {

        StageGold = 0;
        _stageInventoryDic.Clear();
        _currentLevelUpRewardDic.Clear();
        Player.Inst.ResetAddedPlayerStageStat();
    }
    public void AddToRealInventory(float scalef)
    {

        GameManager.Inst.AddGold(StageGold);
        foreach (KeyValuePair<string, int> item in _stageInventoryDic)
        {
            GameManager.Inst.AddInventory(item.Key, (int)(item.Value * scalef));
        }
    }
    public void PlayerGetExp(int expAmount)
    {
        PlayerExp = PlayerExp + expAmount;
        while (PlayerExp >= _maxExp)
        {
            PlayerExp = PlayerExp - _maxExp;
            _maxExp = Mathf.RoundToInt(_maxExp * 1.2f);
            _levelUpQueue.Enqueue(true);
        }
        OnExpChanged?.Invoke(PlayerExp, _maxExp, PlayerLevel);
        CheckAndProcessLevelUpQueue();
    }

    private void CheckAndProcessLevelUpQueue()
    {
        if (_isLevelUpUI || _levelUpQueue.Count == 0) return;

        _levelUpQueue.Dequeue();

        _isLevelUpUI = true;
        PlayerLevel++;
        GetLevelUpReward();
    }
    public void FinishLevelUpReward()
    {
        UIManager.Inst.CloseLevelUpRewardUI();
        GameManager.Inst.ResumeGame();
        OnExpChanged?.Invoke(PlayerExp, _maxExp, PlayerLevel);
        _isLevelUpUI = false;
        CheckAndProcessLevelUpQueue();
    }
    public void ResetLevel()
    {
        PlayerLevel = 0;
        PlayerExp = 0;
        _maxExp = 300;
    }
    public void GetLevelUpReward()
    {
        GameManager.Inst.PauseGame();
        _finalRewardList.Clear();
        _finalRewardList = GetRandomLevelUpReward(3);
        UIManager.Inst.OpenLevelUpRewardUI();
    }
    public void InitCurrentLevelUpRewardDic()
    {
        foreach (KeyValuePair<string, LevelUpRewardData> reward in DataManager.Inst.LevelUpRewardDataList)
        {
            _currentLevelUpRewardDic.Add(reward.Key, 0);
        }
    }
    public List<string> GetRandomLevelUpReward(int count)
    {

        List<string> finalReward = new List<string>();

        foreach (KeyValuePair<string, LevelUpRewardData> data in DataManager.Inst.LevelUpRewardDataList)
        {
            if (!(CheckRewardIsMaxLevel(data.Key)))
            {
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
    public bool CheckRewardIsMaxLevel(string id)
    {
        LevelUpRewardData data = DataManager.Inst.GetLevelUpRewardData(id);

        if (_currentLevelUpRewardDic[id] >= data.MaxLevel) return true;
        else return false;
    }
    public void PlayerGetLevelUpReward(string id)
    {
        Debug.Log($"레벨업 할 스텟{id}");
        _currentLevelUpRewardDic[id]++;
        PlayerStatLevelUp(id);
        OnPlayerStatChanged?.Invoke();
    }
    public void PlayerStatLevelUp(string id)
    {
        LevelUpRewardData data = DataManager.Inst.GetLevelUpRewardData(id);
        RewardStatType type = data.StatType;
        Debug.Log($"레벨업 할 스텟{type}");
        switch (type)
        {
            case RewardStatType.MaxHp: PlayerMaxHpLevelUp(data); Debug.Log($"변경후 스텟{Player.Inst._stageAddMaxHpPercent}"); break;
            case RewardStatType.MoveSpeed: Player.Inst._stageAddMoveSpeed += data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddMoveSpeed}"); break;
            case RewardStatType.Attack: Player.Inst._stageAddAttack += (int)data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddAttack}"); break;
            case RewardStatType.MagnetRange: Player.Inst._stageAddMagnetRange += data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddMagnetRange}"); break;
            case RewardStatType.Armor: Player.Inst._stageAddArmor += (int)data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddArmor}"); break;
            case RewardStatType.CriticalPercent: Player.Inst._stageAddCriticalPercent += (int)data.Value; Debug.Log($"변경후 스텟{Player.Inst._stageAddCriticalPercent}"); break;
            default: break;
        }
    }
    public void PlayerMaxHpLevelUp(LevelUpRewardData data)
    {
        Player.Inst._stageAddMaxHpPercent += data.Value;
        Player.Inst.GetLevelUpHp(data.Value);
    }

    public void UseItemToKey(int key)
    {
        var inventoryDic = GameManager.Inst._inventoryDic;
        if (inventoryDic == null || inventoryDic.Count == 0) return;
        if (key < 0) return;

        List<string> usableItemIdList = new List<string>();
        foreach (string itemId in inventoryDic.Keys)
        {
            if (DataManager.Inst.GetItemData(itemId).IsUseable)
            {
                usableItemIdList.Add(itemId);
            }
        }
        if (key >= usableItemIdList.Count) return;
        string targetItemId = usableItemIdList[key];

        ItemType type = DataManager.Inst.GetItemData(targetItemId).EItemType;

        switch (type)
        {
            case ItemType.HpPotion:
                if (Player.Inst.UseHpPotion() && inventoryDic[targetItemId] > 0)
                {
                    inventoryDic[targetItemId]--;
                    if (inventoryDic[targetItemId] <= 0)
                    {
                        inventoryDic.Remove(targetItemId);
                    }
                    OnUseableItemChanged?.Invoke();
                }
                break;
            case ItemType.MagnetPotion:
                if (inventoryDic[targetItemId] > 0)
                {
                    _dropItemSpawner.StartMagnetToAllDropItems(Player.Inst.transform);
                    inventoryDic[targetItemId]--;
                    if (inventoryDic[targetItemId] <= 0)
                    {
                        inventoryDic.Remove(targetItemId);
                    }
                    OnUseableItemChanged?.Invoke();
                }
                break;
            case ItemType.BerserkPotion:
                if (Player.Inst.UseFireSpeedPotion(10, 5) && inventoryDic[targetItemId] > 0)
                {
                    inventoryDic[targetItemId]--;
                    if (inventoryDic[targetItemId] <= 0)
                    {
                        inventoryDic.Remove(targetItemId);
                    }
                    OnUseableItemChanged?.Invoke();
                }
                break;
            default: break;
        }

    }
    public void SetSpawners(int stageNum)
    {

        var row = DataManager.Inst.StageMonsterTableDataList.ElementAt(stageNum);

        _monsterSpawmer1.InitMonsterSpawner(Player.Inst.transform, row.Value.MonsterId1);
        _monsterSpawmer2.InitMonsterSpawner(Player.Inst.transform, row.Value.MonsterId2);
        _dropItemSpawner.InitDropItemSpawner();
        _bulletSpawner.InitBulletSpawner();
        _bombSpawner.InitBombSpawner();
    }
    public void StartSpawners() 
    {
        _monsterSpawmer1.StartSpawn();
        _monsterSpawmer2.StartSpawn();
    }
    public void OnlySetSpawners() 
    {
        _monsterSpawmer1.OnlySetSpawn();
        _monsterSpawmer2.OnlySetSpawn();    
    }
    public void ResumeSpawners() 
    {
        _monsterSpawmer1.ResumeSpawn();
        _monsterSpawmer2.ResumeSpawn();
    }
    public void StopSpawners() 
    {
        _monsterSpawmer1.StopSpawn();
        _monsterSpawmer2.StopSpawn();
    }
    public void ClearSpawners() 
    {
        _monsterSpawmer1.ClearAndReleaseSpawner();
        _monsterSpawmer2.ClearAndReleaseSpawner();
        _dropItemSpawner.ClearAndReleaseSpawner();
        _bulletSpawner.ClearAndReleaseSpawner();
        _bulletSpawnerForBoss.ClearAndReleaseSpawner();
        _bombSpawner.ClearAndReleaseSpawner();
    }
    public async UniTaskVoid StartBossBattle() 
    {
        MonsterData bossData = DataManager.Inst.GetMonsterData("monster_boss_01");
        string bossPrefabAddress = bossData.PrefabPath;
        GameObject boss = await SpawnObjectWithAddress(bossPrefabAddress,new Vector3(0f,30f,0f));
        if (boss == null)
        {
            Debug.LogError($"보스 프리팹 생성 실패: {bossPrefabAddress}");
            return;
        }
        _targetBoss = boss.GetComponent<Monster>();
        if (_targetBoss != null)
        {
            _targetBoss.InitMonster(bossData, 1, 1, false);
            Debug.Log("보스 몬스터 스텟 초기화 완료!");
        }
        else
        {
            Debug.LogError("생성된 보스 프리팹 최상위에 'Monster' 스크립트가 붙어있지 않습니다!");
        }
    }
    public async UniTask<GameObject> SpawnObjectWithAddress(string address, Vector3 position)
    {
        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(address).ToUniTask();
        if (prefab == null) return null;

        return Instantiate(prefab, position, Quaternion.identity);
    }
}
