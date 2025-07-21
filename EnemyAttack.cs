using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.2f;
    public float extraRangeBuffer = 1.25f; // Буфер дистанции между врагом и игроком
    public float attackCooldown = 2f;
    public int damage = 1;
    public Animator animator;

    private float lastAttackTime = 0f;
    private Transform player;
    private PlayerHealth playerHealth;
    private PlayerStats playerStats;
    private EnemyHealth self;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerStats = player.GetComponent<PlayerStats>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        self = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (player == null || playerHealth == null || playerHealth.IsDead || self == null || self.IsDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange + extraRangeBuffer && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            Invoke(nameof(DealDamage), 0.2f);
            lastAttackTime = Time.time;
        }
    }

    void DealDamage()
    {
        if (playerHealth != null && !playerHealth.IsDead && self != null)
        {
            int baseDamage = self.damage;

            int defense = playerStats != null ? playerStats.bonusDefenseFromItems : 0;
            int finalDamage = Mathf.CeilToInt(baseDamage * (100f / (100f + defense)));

            playerHealth.TakeDamage(finalDamage);
        }
    }
}