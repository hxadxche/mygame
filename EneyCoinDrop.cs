using UnityEngine;

public class EnemyCoinDrop : MonoBehaviour
{
    [SerializeField] private int baseMinCoins = 1;
    [SerializeField] private int baseMaxCoins = 3;

    private int currentFloor = 1;

    void Start()
    {
        // �������� ������� ���� �� LevelManager
        currentFloor = LevelManager.Instance != null ? LevelManager.Instance.CurrentFloor : 1;
    }

    public int GetCoinDrop()
    {
        float multiplier = 1f + (currentFloor - 1) * 0.2f; // �� 20% ������ �� ������ ����
        int min = Mathf.RoundToInt(baseMinCoins * multiplier);
        int max = Mathf.RoundToInt(baseMaxCoins * multiplier);
        return Random.Range(min, max + 1);
    }
}