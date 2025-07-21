using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public int minEnemies = 3;
    public int maxEnemies = 4;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private bool waveSpawning = false;

    void Update()
    {
        currentEnemies.RemoveAll(enemy => enemy == null);

        if (currentEnemies.Count == 0 && !waveSpawning)
        {
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        waveSpawning = true;
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            float randomX = Random.Range(player.position.x + 20f, player.position.x + 40f);
            float spawnY = -2.075f;
            Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            Vector3 scale = enemy.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1f;
            enemy.transform.localScale = scale;

            if (!enemy.TryGetComponent<EnemyHealth>(out var health))
                health = enemy.AddComponent<EnemyHealth>();

            health.maxHealth = 5;
            health.animator = enemy.GetComponent<Animator>();

            // >>> ”—“¿ÕŒ¬ ¿ ”–Œ¬Õﬂ »« LEVEL MANAGER <<<
            SetEnemyLevel(health);

            Debug.Log($"[¬–¿√] —Ô‡‚Ì: {enemy.name}, ÔÓÁËˆËˇ X = {randomX}, Y = {spawnY}");

            currentEnemies.Add(enemy);
            yield return null;
        }

        waveSpawning = false;
    }

    private void SetEnemyLevel(EnemyHealth enemy)
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            int lvl = Random.Range(levelManager.enemyLevelMin, levelManager.enemyLevelMax + 1);
            enemy.level = lvl;
        }
    }
}
