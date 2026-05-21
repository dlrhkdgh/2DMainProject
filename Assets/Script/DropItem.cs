using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _moveSpeed = 8f;
    public string ItemId { get; set; }
    public int GoldAmount { get; set; } = 0;
    private Transform _targetTransform;
    private Coroutine _flyCoroutine;

   
    public void StartAttracting(Transform target)
    {
        //Debug.Log("atrrct strat");
        if (_flyCoroutine != null) return;

        _targetTransform = target;
        _flyCoroutine = StartCoroutine(MoveToPlayerCo());
    }

    private IEnumerator MoveToPlayerCo()
    {
        while (_targetTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetTransform.position, _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (_flyCoroutine != null)
        {
            StopCoroutine(_flyCoroutine);
            _flyCoroutine = null;
        }
        _targetTransform = null;

        ItemId = string.Empty;
        GoldAmount = 0;
        _spriteRenderer.sprite = null;
    }

    
    public async UniTaskVoid InitDroppedItemAsync(ItemData data)
    {
        if (data == null) return;

        ItemId = data.Id;
        GoldAmount = 0;
        await LoadAndApplySprite(data);
    }
    public async UniTaskVoid InitDroppedItemAsync(ItemData data,int goldAmount)
    {
        if (data == null) return;

        ItemId = data.Id;
        GoldAmount= goldAmount;
        await LoadAndApplySprite(data);
    }
    private async UniTask LoadAndApplySprite(ItemData data)
    {
        Sprite itemTexture = await ResourceManager.Inst.LoadSprite(data.IconPath);
        if (itemTexture != null)
        {
            _spriteRenderer.sprite = itemTexture;
        }
        else
        {
            Debug.LogWarning($"[{data.Name}]의 아이콘을 로드하지 못해 기본 이미지를 유지합니다.");
        }
    }
}
