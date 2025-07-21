using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Инфо врага")]
    public string enemyName = "Скелет";
    public int level = 1;

    [Header("Базовые значения")]
    public int baseMaxHealth = 5;
    public int baseDamage = 1;

    [Header("Характеристики")]
    public int maxHealth = 5;
    public int CurrentHealth { get; private set; }
    public int damage = 1;

    public Animator animator;
    public bool IsDead { get; private set; } = false;

    void Start()
    {
        maxHealth = baseMaxHealth + level * 2;
        damage = baseDamage + Mathf.FloorToInt(level * 0.5f);
        CurrentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;

        IsDead = true;

        // >>> Выдача XP игроку <<<
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.GainXP(3);
            }
        }

        // >>> Уведомление LevelManager <<<
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
            levelManager.RegisterKill();

        // >>> Дроп предметов <<<
        EnemyDropHandler dropper = GetComponent<EnemyDropHandler>();
        if (dropper != null)
        {
            dropper.DropItems();
        }

        // >>> Дроп монет <<<
        EnemyCoinDrop coinDrop = GetComponent<EnemyCoinDrop>();
        if (coinDrop != null)
        {
            int coins = coinDrop.GetCoinDrop();

            if (PlayerResources.Instance != null)
            {
                PlayerResources.Instance.AddCoins(coins);
            }
            else
            {
                Debug.LogWarning("PlayerResources.Instance == null при дропе монет");
            }
        }

        // Анимация смерти
        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Die");
            animator.Play("Die", 0, 0f);
        }

        // Отключение физики
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        float dieTime = GetAnimationClipLength("Die");
        Destroy(gameObject, dieTime);
        PassiveEffectManager.Trigger(PassiveTrigger.OnKillEnemy, gameObject);

    }

    private float GetAnimationClipLength(string clipName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return 1f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        return 1f;
    }
}