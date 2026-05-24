using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Inst { get; set; }
    [SerializeField] BulletSpawner _bulletSpawner;
    [SerializeField] MonsterSpawner _monsterSpawmer1;
    [SerializeField] MonsterSpawner _monsterSpawmer2;
    [SerializeField] DropItemSpawner _dropItemSpawner;
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
    public void StartStage(int stageNum) {
        _monsterSpawmer1.InitMonsterSpawner(Player.Inst.transform);
        _monsterSpawmer2.InitMonsterSpawner(Player.Inst.transform);
        _dropItemSpawner.InitDropItemSpawner();
        _bulletSpawner.InitBulletSpawner();
        Player.Inst.StartShooting();
    }
    public void DropItemFromMonster(Vector3 diePosition, string monsterDropTableId) {
        
        _dropItemSpawner.DropItemInField(diePosition, monsterDropTableId);
      
    }
    public void StartFireBullet(Vector3 spawnPosition, Vector2 direction) {
        _bulletSpawner.FireBullet(spawnPosition, direction);
    }
    public void FinishStage()
    {
        _monsterSpawmer1.ClearAndReleaseSpawner();
        _monsterSpawmer2.ClearAndReleaseSpawner();
        _dropItemSpawner.ClearAndReleaseSpawner();
        _bulletSpawner.ClearAndReleaseSpawner();
        Player.Inst.StopShooting();
    }
}
