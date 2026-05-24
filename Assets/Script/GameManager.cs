using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst { get; private set; }
    public int Gold { get; set; } = 0;
    public Dictionary<string, int> _inventoryDic = new Dictionary<string, int>();
    private void Awake()
    {
        Inst = this;
    }
    void Start()
    {
        GameStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GameStart() {

        UIManager.Inst.GameStartUI();
    }
    public bool AddGold(int getGold) {

        int newGold = Gold + getGold;
        if (newGold<0)
        {
            return false;
        }
        else {
        Gold= newGold;
            return true;
        
        }
    }
    public void AddInventory(string itemId, int itemCount) {

        if (string.IsNullOrEmpty(itemId) || itemCount == 0) return;

        if (_inventoryDic.ContainsKey(itemId))
        {
            _inventoryDic[itemId] = _inventoryDic[itemId] + itemCount;
            //DebugPrintInventory();
        }
        else {
        _inventoryDic.Add(itemId, itemCount);
            //DebugPrintInventory();
        }
    }
    public void DebugPrintInventory()
    {
        if (_inventoryDic == null || _inventoryDic.Count == 0)
        {
            return;
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("============  CURRENT INVENTORY LIST ============");
        sb.AppendLine($"총 아이템 종류 수: {_inventoryDic.Count}종");
        sb.AppendLine("--------------------------------------------------");

       
        foreach (KeyValuePair<string, int> item in _inventoryDic)
        {
            sb.AppendLine($"▶ ID: {item.Key,-15} | 수량: {item.Value}개");
        }

        sb.AppendLine("==================================================");

        Debug.Log(sb.ToString());
    }
    public void StartStage(int stageNum) 
    {
        StageManager.Inst.StartStage(stageNum);
        UIManager.Inst.OpenInFeildUI();
    }
    public void FinishStage() 
    {
        StageManager.Inst.FinishStage();
        UIManager.Inst.CloseInFeildUI();
    }
}
