using System;
using UnityEngine;

public enum PassiveEffectType
{
    ExtraXP,       // ���������� XP �� ������
    BonusCoins,    // ���� �������� ������ ��� ��������
    Thorns,        // ��������� ����� ����� ��� ��������� �����
    Lifesteal,     // ��������� �������������� HP ��� ����� �����
    BonusCrit      // ����������� ���� ������������ �����
}

public enum PassiveTrigger
{
    OnEquip,
    OnUnequip,
    OnHitEnemy,
    OnKillEnemy,
    OnTakeDamage
}

[CreateAssetMenu(fileName = "New Passive Effect", menuName = "Items/Passive Effect")]
public class PassiveEffectData : ScriptableObject
{
    public string effectName;
    [TextArea(2, 5)] public string description;
    public PassiveTrigger trigger;

    [Header("���������")]
    public float chance = 1f;
    public int value = 0;
    public string extraTag;

    public PassiveEffectType effectType;
}
