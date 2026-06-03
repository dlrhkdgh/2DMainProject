using UnityEngine;

public class TileMapLoop : MonoBehaviour
{
    private Transform _playerTransform;

     private float _chunkSizeX = 73f;
     private float _chunkSizeY = 40f;

    void Start()
    {
        if (Player.Inst != null)
        {
            _playerTransform = Player.Inst.transform;
        }
    }

    void Update()
    {
        if (_playerTransform == null) return;

        
        float distanceX = _playerTransform.position.x - transform.position.x;
        float distanceY = _playerTransform.position.y - transform.position.y;

        
        if (Mathf.Abs(distanceX) >= _chunkSizeX * 1.5f)
        {
            float moveDirectionX = Mathf.Sign(distanceX);
            Vector3 newPosition = transform.position;
            newPosition.x += moveDirectionX * _chunkSizeX * 3f; 
            transform.position = newPosition;
        }
        if (Mathf.Abs(distanceY) >= _chunkSizeY * 1.5f)
        {
            float moveDirectionY = Mathf.Sign(distanceY);
            Vector3 newPosition = transform.position;
            newPosition.y += moveDirectionY * _chunkSizeY * 3f;
            transform.position = newPosition;
        }
    }
}
