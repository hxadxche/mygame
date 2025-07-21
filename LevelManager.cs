using UnityEngine;
using System.Collections;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Этажи и прогресс")]
    public int currentLevel = 1;
    public int baseKillsToAdvance = 10;
    public int killsToAdvance = 10;
    private int killCount = 0;

    public int CurrentFloor => currentLevel; // 🔥 теперь можно вызывать LevelManager.Instance.CurrentFloor

    [Header("Уровни врагов")]
    public int enemyLevelMin = 1;
    public int enemyLevelMax = 1;

    [Header("UI и эффекты")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.5f;
    public CanvasGroup levelTextGroup;
    public TextMeshProUGUI levelText;
    public float levelTextFadeDuration = 0.5f;

    private Transform playerTransform;
    private Vector3 startingPosition;

    public delegate void OnLevelChange(int newLevel);
    public static event OnLevelChange LevelChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        killsToAdvance = baseKillsToAdvance;

        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;

        if (levelTextGroup != null)
            levelTextGroup.alpha = 0f;

        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform != null)
            startingPosition = playerTransform.position;

        if (currentLevel == 1)
        {
            enemyLevelMin = 1;
            enemyLevelMax = 1;
        }
        else
        {
            UpdateEnemyLevelRange();
        }

        // Установить фоны для первого этажа
        var bgSwitcher = FindObjectOfType<BackgroundSwitcher>();
        if (bgSwitcher != null)
            bgSwitcher.SetBackgroundsForFloor(currentLevel);
    }

    public void RegisterKill()
    {
        killCount++;

        if (killCount >= killsToAdvance)
        {
            StartCoroutine(HandleLevelTransition());
        }
    }

    private IEnumerator HandleLevelTransition()
    {
        yield return StartCoroutine(FadeTo(1f));

        // Удаление всех врагов
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }

        currentLevel++;
        killCount = 0;
        killsToAdvance = baseKillsToAdvance + (currentLevel * 5);
        UpdateEnemyLevelRange();

        if (playerTransform != null)
            playerTransform.position = startingPosition;

        BackgroundResetter[] backgrounds = FindObjectsOfType<BackgroundResetter>();
        foreach (var bg in backgrounds)
        {
            bg.ResetPosition();
        }

        var bgSwitcher = FindObjectOfType<BackgroundSwitcher>();
        if (bgSwitcher != null)
            bgSwitcher.SetBackgroundsForFloor(currentLevel);

        LevelChanged?.Invoke(currentLevel);

        if (levelText != null)
            levelText.text = $"ЭТАЖ {currentLevel}";

        if (levelTextGroup != null)
            StartCoroutine(ShowLevelText());

        Debug.Log("ЭТАЖ " + currentLevel);
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeTo(0f));
    }

    private IEnumerator ShowLevelText()
    {
        float time = 0f;

        while (time < levelTextFadeDuration)
        {
            time += Time.deltaTime;
            levelTextGroup.alpha = Mathf.Lerp(0f, 1f, time / levelTextFadeDuration);
            yield return null;
        }

        levelTextGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);

        time = 0f;
        while (time < levelTextFadeDuration)
        {
            time += Time.deltaTime;
            levelTextGroup.alpha = Mathf.Lerp(1f, 0f, time / levelTextFadeDuration);
            yield return null;
        }

        levelTextGroup.alpha = 0f;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
            yield break;
        float startAlpha = fadeCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    private void UpdateEnemyLevelRange()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                int playerLvl = stats.currentLevel;
                enemyLevelMin = playerLvl + 2;
                enemyLevelMax = playerLvl + 4;
            }
        }
    }
}