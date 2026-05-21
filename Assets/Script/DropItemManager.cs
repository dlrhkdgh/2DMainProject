using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DropItemManager : MonoBehaviour
{
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private Coin _coinPrefab;
    
    [SerializeField] private AssetReference _coinAddressableRef;
    public static DropItemManager Inst { get; private set; }
    private string _coinId = "item_coin_01";
    private string _coinAddressKey;
    private List<Coin> _coinPool = new List<Coin>();
    private int _currentPivot = 0;
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        var itemDic = DataManager.Inst.GetItemData(_coinId);
        _coinAddressKey = itemDic.PrefabPath;
        AsyncMonsterPool().Forget();
        
    }
    
    private async UniTaskVoid AsyncMonsterPool()
    {

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject coinResource = await ResourceManager.Inst.InstantiateAsync(_coinAddressKey, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

            if (coinResource != null)
            {
                Coin coin = coinResource.GetComponent<Coin>();
                coin.gameObject.SetActive(false);
                _coinPool.Add(coin);
            }
        }
      
    }
    
    public void DropItemInField(Vector3 dropPosition, string itemId)
    {
        Coin cointToDrop = null;

        for (int i = 0; i < _coinPool.Count; i++)
        {
            int checkIndex = (_currentPivot + i) % _coinPool.Count;
            if (!_coinPool[checkIndex].gameObject.activeSelf)
            {
                cointToDrop = _coinPool[checkIndex];
                _currentPivot = (checkIndex + 1) % _coinPool.Count;
                break;
            }
        }
        if (cointToDrop != null)
        {
            if (!int.TryParse(itemId, out int itemPrice))
            {
                Debug.LogError($"[DropItemManager] itemId 변환 실패: {itemId}");
                return;
            }


            cointToDrop.Price = itemPrice;
           
            cointToDrop.transform.position = dropPosition;
            cointToDrop.gameObject.SetActive(true);

        }
        else
        {
            Debug.LogWarning("코인 풀이 가득 찼습니다!");
        }
    }
    public void GetDropItem(string itemId) {

      
    }
}
