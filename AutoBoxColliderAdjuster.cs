using UnityEngine;

/// <summary>
/// Автоматически настраивает BoxCollider2D под текущий спрайт объекта.
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
            Debug.LogWarning($"[AutoBoxColliderAdjuster] У объекта {gameObject.name} не задан спрайт!");
            return;
        }

        // Получаем границы текущего спрайта
        Bounds spriteBounds = spriteRenderer.sprite.bounds;

        // Устанавливаем размер и смещение коллайдера
        boxCollider.size = spriteBounds.size;
        boxCollider.offset = spriteBounds.center;
    }
}