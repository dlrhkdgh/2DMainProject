using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DropItemManager : MonoBehaviour
{
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private DropItem _dropPrefab;
    
    [SerializeField] private string dropItemPrefabPath;
    public static DropItemManager Inst { get; private set; }
    private string _coinId = "item_coin_01";
    private string _dropItemAddressKey;
    private List<DropItem> _dropItemPool = new List<DropItem>();
    private DropTableData _dropTableData;
    private int _currentPivot = 0;
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
       // var itemDic = DataManager.Inst.GetItemData(_coinId);
       // _dropItemAddressKey = itemDic.PrefabPath;
        AsyncMonsterPool().Forget();
        
    }
    
    private async UniTaskVoid AsyncMonsterPool()
    {

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject dropItemResource = await ResourceManager.Inst.InstantiateAsync(dropItemPrefabPath, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

            if (dropItemResource != null)
            {
                DropItem dropItem = dropItemResource.GetComponent<DropItem>();
                dropItem.gameObject.SetActive(false);
                _dropItemPool.Add(dropItem);
            }
        }
      
    }
    
    public void DropItemInField(Vector3 dropPosition, string dropTableId)
    {
        DropItem dropItemtToDrop = null;
        _dropTableData=DataManager.Inst.GetDropTableData(dropTableId);
        for (int i = 0; i < _dropItemPool.Count; i++)
        {
            int checkIndex = (_currentPivot + i) % _dropItemPool.Count;
            if (!_dropItemPool[checkIndex].gameObject.activeSelf)
            {
                dropItemtToDrop = _dropItemPool[checkIndex];
                _currentPivot = (checkIndex + 1) % _dropItemPool.Count;
                break;
            }
        }
        if (dropItemtToDrop != null)
        {
            
            dropItemtToDrop.transform.position = dropPosition;
            //dropItemtToDrop.InitDroppedItemAsync(dropTableId).Forget();
            dropItemtToDrop.gameObject.SetActive(true);

        }
        else
        {
            Debug.LogWarning("코인 풀이 가득 찼습니다!");
        }
    }
    public void GetDropItem(string itemId) {

      
    }
}
