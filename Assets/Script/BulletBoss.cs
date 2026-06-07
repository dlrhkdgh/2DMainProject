using UnityEngine;

public class BulletBoss : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            if (collision.TryGetComponent<Player>(out var player))
            {
                player.TakeDamage(BulletDamage); 
            }
            DestroyBullet();
        }
    }
}
