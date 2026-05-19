using Unity.VisualScripting;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    [SerializeField] private Coin _coinPrefab;
    public static DropItemManager Inst { get; private set; }

    private void Awake()
    {
        Inst = this;
    }
    public void DropItemInField(Vector3 dropPosition, string itemId) {
       
        if (!int.TryParse(itemId, out int itemPrice))
        {
            Debug.LogError($"[DropItemManager] itemId 변환 실패: {itemId}");
            return;
        }
        Coin spawnedCoin = Instantiate(_coinPrefab, dropPosition, Quaternion.identity, transform);

        if (spawnedCoin != null)
        {
            spawnedCoin.Price = itemPrice;
        }
        

    }
    public void GetDropItem(string itemId) {

      
    }
}
