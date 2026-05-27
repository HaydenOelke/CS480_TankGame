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

        BasicEnemyController enemy = other.GetComponent<BasicEnemyController>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}