using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    public int maxHealth = 3;
    public EnemyHealthBar healthBar;

    private int currentHealth;

    [Header("Detection")]
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Combat")]
    public int attackDamage = 1;
    public float attackCooldown = 1f;

    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;
    private float attackTimer = 0f;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<EnemyHealthBar>();
        }

        if (healthBar != null)
        {
            healthBar.SetFill(1f);
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("BasicEnemyController: No GameObject tagged 'Player' found in scene.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        attackTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (distToPlayer <= detectionRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (distToPlayer > detectionRange)
                    currentState = State.Idle;
                else if (distToPlayer <= attackRange)
                    currentState = State.Attack;
                else
                    MoveTowardPlayer();
                break;

            case State.Attack:
                if (distToPlayer > attackRange)
                    currentState = State.Chase;
                else
                    TryAttack();
                break;
        }
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    void TryAttack()
{
    if (attackTimer <= 0f)
    {
        Debug.Log("Enemy attacks! Distance: " + Vector3.Distance(transform.position, player.position));
        player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        attackTimer = attackCooldown;
    }
}

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (healthBar != null)
        {
            healthBar.SetFill(Mathf.Clamp01((float)currentHealth / maxHealth));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        WaveManager wm = FindAnyObjectByType<WaveManager>();
        if (wm != null)
        {
            wm.EnemyDefeated();
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}