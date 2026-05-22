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
        if (Input.GetButtonDown("Jump")) {

            StageStart();
        }
    }
    void StageStart() {
        _monsterSpawmer1.InitMonsterSpawner(Player.Inst.transform);
        _monsterSpawmer2.InitMonsterSpawner(Player.Inst.transform);
        _dropItemSpawner.InitDropItemSpawner();
        _bulletSpawner.InitBulletSpawner();
    }
    public void DropItemFromMonster(Vector3 diePosition, string monsterDropTableId) {
        
        _dropItemSpawner.DropItemInField(diePosition, monsterDropTableId);
      
    }
    public void StartFireBullet(Vector3 spawnPosition, Vector2 direction) {
        _bulletSpawner.FireBullet(spawnPosition, direction);
    }

}
