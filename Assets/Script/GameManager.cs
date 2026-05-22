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
        }
        else {
        _inventoryDic.Add(itemId, itemCount);        
        }
    }
    public void DebugPrintInventory()
    {
        // 1. 인벤토리가 완전히 비어있을 때 예외 처리 (널체크 개념)
        if (_inventoryDic == null || _inventoryDic.Count == 0)
        {
            Debug.Log("<color=yellow> [인벤토리 디버그] 현재 가방이 텅 비어 있습니다.</color>");
            return;
        }

        // 2. 문자열을 효율적으로 합쳐줄 StringBuilder 생성 (유니티 로그 최적화)
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("============  CURRENT INVENTORY LIST ============");
        sb.AppendLine($"총 아이템 종류 수: {_inventoryDic.Count}종");
        sb.AppendLine("--------------------------------------------------");

        // 3.  [핵심] foreach문을 이용해 딕셔너리의 모든 Key-Value 쌍을 순회합니다.
        foreach (KeyValuePair<string, int> item in _inventoryDic)
        {
            // item.Key는 아이템 ID ("item_potion_01"), item.Value는 개수 (5)가 들어있습니다.
            sb.AppendLine($"▶ ID: {item.Key,-15} | 수량: {item.Value}개");
        }

        sb.AppendLine("==================================================");

        // 4. 조립된 전체 문자열을 콘솔창에 단 한 번의 로그로 깔끔하게 출력!
        Debug.Log(sb.ToString());
    }
}
