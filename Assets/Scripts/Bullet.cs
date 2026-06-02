using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public string ownerTag = "Player";

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        if (!string.IsNullOrEmpty(ownerTag) && other.CompareTag(ownerTag))
            return;

        if (other.CompareTag("Bullet"))
            return;

        if (other.CompareTag("Untagged"))
            return;

        // Hit enemy
        BasicEnemyController enemy = other.GetComponent<BasicEnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Hit boss
        BossEnemyController boss = other.GetComponent<BossEnemyController>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Hit player
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}