using UnityEngine;

public class BossEnemyController : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;

    [Header("Detection")]
    public float detectionRange = 100f;
    public float attackRange = 15f;
    public float moveSpeed = 4f;

    [Header("Combat")]
    public int attackDamage = 20;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    private Transform player;
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (dist <= detectionRange) currentState = State.Chase;
                break;
            case State.Chase:
                if (dist <= attackRange) currentState = State.Attack;
                else MoveTowardPlayer();
                break;
            case State.Attack:
                if (dist > attackRange) currentState = State.Chase;
                else TryAttack();
                break;
        }
    }

    void MoveTowardPlayer()
{
    Vector3 dir = (player.position - transform.position).normalized;
    dir.y = 0;
    
    // Smooth rotation
    Quaternion targetRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, 90, 0);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    
    transform.position += dir * moveSpeed * Time.deltaTime;
}

    void TryAttack()
    {
        if (attackTimer <= 0f)
        {
            ShootAtPlayer();
            attackTimer = attackCooldown;
        }
    }

    void ShootAtPlayer()
{
    if (bulletPrefab == null || firePoint == null) return;

    Vector3 dir = (player.position - firePoint.position).normalized;
    dir.y = 0;

    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));

    Bullet b = bullet.GetComponent<Bullet>();
    if (b != null)
    {
        b.ownerTag = "Enemy";
        b.damage = attackDamage;
    }

    Rigidbody rb = bullet.GetComponent<Rigidbody>();
    if (rb != null)
        rb.linearVelocity = dir * bulletSpeed;

    Destroy(bullet, 5f);
}

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }
}

