using UnityEngine;

/// <summary>
/// ������������� ����������� BoxCollider2D ��� ������� ������ �������.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class AutoBoxColliderAdjuster : MonoBehaviour
{
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (spriteRenderer.sprite == null)
        {
            Debug.LogWarning($"[AutoBoxColliderAdjuster] � ������� {gameObject.name} �� ����� ������!");
            return;
        }

        // �������� ������� �������� �������
        Bounds spriteBounds = spriteRenderer.sprite.bounds;

        // ������������� ������ � �������� ����������
        boxCollider.size = spriteBounds.size;
        boxCollider.offset = spriteBounds.center;
    }
}